var boton = document.getElementById('Boton');
var fileName = document.getElementById('fileName')
var imagen = document.getElementById('imagen')

boton.onchange = (e) => {
    let reader = new FileReader();
    reader.readAsDataURL(boton.files[0]);
    console.log(boton.files[0]);
    reader.onload = () => {
        imagen.setAttribute("src", reader.result)
    }
    reader.onload = () => {
        fileName.textContent = fileName.textContent + boton.files[0].name;
    }
}
