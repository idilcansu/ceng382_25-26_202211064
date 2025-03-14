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

// Çift Tıklama Olayı
classTable.addEventListener('dblclick', function (e) {
    if (e.target.tagName === 'TD') {
        const row = e.target.parentElement;
        const className = row.cells[0].innerText;
        const numPeople = row.cells[1].innerText;
        const description = row.cells[2].innerText;

        console.log('Çift Tıklama Satır Detayları:', { className, numPeople, description });
        // İsteğe bağlı olarak satırı kaldırabilirsiniz:
        // row.remove();
    }
});