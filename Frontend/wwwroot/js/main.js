function loadForm(formName) {
    const container = document.getElementById('form-container');
    document.getElementById('welcome-message').style.display = 'none';

    fetch(`/forms/${formName}.html`)
        .then(response => {
            if (!response.ok) throw new Error("Erro ao carregar formulário");
            return response.text();
        })
        .then(html => {
            container.innerHTML = html;
        })
        .catch(error => {
            container.innerHTML = `<p style="color:red;">Erro: ${error.message}</p>`;
        });
}
