let userLogins = [];

function showLogin() {
    let loginForm = document.getElementById('loginForm');
    loginForm.style.display = 'block';
    loginForm.style.animation = 'slideFadeIn 0.6s ease-out forwards'; //animasyon geçişinde chatten yardım alındı
    document.getElementById('enterButton').style.display = 'none';
}

function storeLogin() {
    let username = document.getElementById('username').value;
    let password = document.getElementById('password').value;
    
    if (username && password) {
        userLogins.push({ username, password });
        console.log("User Logins:", userLogins);
        alert("Login successful!");
    } else {
        alert("Please enter both username and password.");
    }
}

function updateClock() {
    let now = new Date();
    let hours = now.getHours().toString().padStart(2, '0');
    let minutes = now.getMinutes().toString().padStart(2, '0');
    let seconds = now.getSeconds().toString().padStart(2, '0');
    document.getElementById('clock').textContent = `${hours}:${minutes}:${seconds}`;
}
setInterval(updateClock, 1000);
updateClock();

//help from chat for clock 
// help from chat for h button

document.addEventListener('keydown', function(event) {
    if (event.key === 'h' || event.key === 'H') {
        let forms = document.querySelectorAll('.login-form');
        forms.forEach(form => {
            form.style.display = (form.style.display === 'none' || form.style.display === '') ? 'block' : 'none';
        });
    }
});
