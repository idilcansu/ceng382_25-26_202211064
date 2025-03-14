const loginForm = document.getElementById('loginForm');
const liveClock = document.getElementById('live-clock');
let users = [];

loginForm.addEventListener('submit', function (e) {
    e.preventDefault();
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    // Kullanıcı adı ve şifreyi diziye ekle
    users.push({ username, password });
    console.log(users);

    // Giriş başarılıysa table.html sayfasına yönlendir
    if (username === 'admin' && password === 'admin') {
        window.location.href = 'table.html';  // table.html sayfasına yönlendirme
    } else {
        alert('Invalid login credentials');
    }
});

// Canlı saat
function updateClock() {
    const currentTime = new Date();
    const hours = String(currentTime.getHours()).padStart(2, '0');
    const minutes = String(currentTime.getMinutes()).padStart(2, '0');
    const seconds = String(currentTime.getSeconds()).padStart(2, '0');
    liveClock.innerText = `${hours}:${minutes}:${seconds}`;
}

setInterval(updateClock, 1000);

// Giriş Alanı Odak Olayı (Input Focus Event)
const loginInputs = document.querySelectorAll('#loginForm input');

loginInputs.forEach(input => {
    input.addEventListener('focus', function () {
        this.style.borderColor = 'blue';
        this.style.boxShadow = '0 0 5px blue';
    });

    input.addEventListener('blur', function () {
        this.style.borderColor = 'red';
        this.style.boxShadow = 'none';
        // Burada giriş verisi doğrulama yapılabilir.
    });
});

// H tuşuna basıldığında login formunu gizle/göster
document.addEventListener('keydown', function(event) {
    if (event.key === 'h') {
        const loginContainer = document.getElementById('loginContainer');
        loginContainer.style.opacity = loginContainer.style.opacity === '0' ? '1' : '0';
    }
});