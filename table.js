const classForm = document.getElementById('classForm');
const classTable = document.getElementById('classTable').getElementsByTagName('tbody')[0];
let classData = [];

classForm.addEventListener('submit', function (e) {
    e.preventDefault();
    const className = document.getElementById('className').value;
    const numPeople = document.getElementById('numPeople').value;
    const description = document.getElementById('description').value;

    // Tabloya veri ekle
    const row = classTable.insertRow();
    row.insertCell(0).innerText = className;
    row.insertCell(1).innerText = numPeople;
    row.insertCell(2).innerText = description;

    classData.push({ className, numPeople, description });
    console.log(classData);
});

// Satır Tıklama Olayı
classTable.addEventListener('click', function (e) {
    if (e.target.tagName === 'TD') {
        const row = e.target.parentElement;
        const className = row.cells[0].innerText;
        const numPeople = row.cells[1].innerText;
        const description = row.cells[2].innerText;

        console.log('Satır Detayları:', { className, numPeople, description });
        row.classList.toggle('highlighted'); // Satırı vurgula
    }
});

// Tablo Tıklama Olayı
document.getElementById('classTable').addEventListener('click', function (e) {
    if (e.target.tagName === 'TABLE') {
        console.log('Tüm Sınıf Girişleri:', classData);
    }
});

// Giriş Alanı Odak Olayı
const formInputs = document.querySelectorAll('#classForm input');

formInputs.forEach(input => {
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

// Fare Üzerinde/Dışında Olayı
classTable.addEventListener('mouseover', function (e) {
    if (e.target.tagName === 'TD') {
        const row = e.target.parentElement;
        row.style.backgroundColor = 'lightyellow';
    }
});

classTable.addEventListener('mouseout', function (e) {
    if (e.target.tagName === 'TD') {
        const row = e.target.parentElement;
        row.style.backgroundColor = ''; // Varsayılan renge dön
    }
});

// H tuşuna basıldığında tabloyu gizle/göster
document.addEventListener('keydown', function(event) {
    if (event.key === 'h') {
        const tableContainer = document.getElementById('tableContainer');
        tableContainer.style.opacity = tableContainer.style.opacity === '0' ? '1' : '0';
    }
});

// Çift Tıklama Olayı (Yeni Eklenen Kod)
classTable.addEventListener('dblclick', function (e) {
    if (e.target.tagName === 'TD') {
        const row = e.target.parentElement;
        const className = row.cells[0].innerText;
        const numPeople = row.cells[1].innerText;
        const description = row.cells[2].innerText;

        console.log('Çift Tıklama Satır Detayları:', { className, numPeople, description });

        // Satırı kaldırma örneği (isteğe bağlı)
        row.remove();
        // Satırı kaldırdıktan sonra classData dizisinden de silmelisiniz.
        classData = classData.filter(item => item.className !== className);
        console.log("Güncellenmiş classData:", classData);

        // Ya da detaylı bilgi gösterme örneği (isteğe bağlı)
        // alert(`Sınıf: ${className}\nKişi Sayısı: ${numPeople}\nAçıklama: ${description}`);
    }
});

// Keyup Olayı (Güncellenmiş Kod)
const classNameInput = document.getElementById('className');
const numPeopleInput = document.getElementById('numPeople');

classNameInput.addEventListener('keyup', function () {
    if (this.value.length < 2) {
        this.title = 'Sınıf adı en az 2 karakter olmalıdır.';
    } else {
        this.title = '';
    }
});

numPeopleInput.addEventListener('keyup', function () {
    if (isNaN(this.value) || this.value < 1) {
        this.title = 'Geçerli bir sayı girin.';
    } else {
        this.title = '';
    }
});