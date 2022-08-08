const btnElm = document.getElementById('btn-click');
const pElm = document.getElementById('contar-click');
var contar = 0;
pElm.textContent = 0;

btnElm.onclick = function () {
    contar++;
    pElm.innerHTML = contar;
}