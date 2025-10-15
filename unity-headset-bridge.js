// Unity BCI Bridge - Sends mental commands to Unity
// Add this to your health-commands-monitor.html

// Unity connection status
let unityConnected = false;
let lastUnityCheck = 0;

// Check Unity connection
async function checkUnityConnection() {
    const now = Date.now();
    if (now - lastUnityCheck < 5000) return unityConnected; // Check every 5 seconds
    
    try {
        const response = await fetch('http://localhost:8080/status', {
            method: 'GET',
            mode: 'cors',
            timeout: 2000
        });
        
        if (response.ok) {
            unityConnected = true;
            console.log('✅ Unity BCI connected');
        } else {
            unityConnected = false;
        }
    } catch (error) {
        unityConnected = false;
        console.log('❌ Unity BCI disconnected');
    }
    
    lastUnityCheck = now;
    return unityConnected;
}

// Command debouncing variables
let lastCommandSent = '';
let lastCommandTime = 0;
let commandQueue = [];
const COMMAND_COOLDOWN = 200; // 200ms between same commands
const STRENGTH_THRESHOLD = 0.4; // Minimum strength to send

// Send command to Unity with debouncing
async function sendCommandToUnity(command, strength, timestamp) {
    // Skip weak commands
    if (strength < STRENGTH_THRESHOLD) {
        return false;
    }
    
    // Skip neutral commands
    if (command === 'neutral') {
        return false;
    }
    
    // Check cooldown for same command
    const now = Date.now();
    if (command === lastCommandSent && (now - lastCommandTime) < COMMAND_COOLDOWN) {
        console.log(`Command throttled: ${command} (too soon)`);
        return false;
    }
    
    if (!await checkUnityConnection()) {
        return false; // Unity not available
    }
    
    const commandData = {
        command: command,
        strength: Math.max(strength, STRENGTH_THRESHOLD), // Ensure minimum strength
        timestamp: timestamp || now
    };
    
    try {
        const response = await fetch('http://localhost:8080/command', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(commandData)
        });
        
        if (response.ok) {
            lastCommandSent = command;
            lastCommandTime = now;
            console.log(`Unity: ${command.toUpperCase()} (${strength.toFixed(2)})`);
            return true;
        } else {
            console.error('Unity command failed:', response.status);
            return false;
        }
    } catch (error) {
        console.error('Unity command error:', error.message);
        unityConnected = false;
        return false;
    }
}

// Initialize Unity connection check
setInterval(checkUnityConnection, 5000);
checkUnityConnection(); // Check immediately