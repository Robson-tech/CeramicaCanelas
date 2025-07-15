// LOG 1: Confirma que o arquivo de script foi carregado e está sendo executado.
console.log('Script js/category.js (padrão similar ao de usuário) EXECUTANDO.');

/**
 * Função principal que inicializa o formulário de categoria.
 */
function initializeCategoryForm(form) {
    if (!form) {
        console.error('FALHA CRÍTICA: Elemento <form class="category-form"> não encontrado.');
        return;
    }

    console.log('🚀 Inicializando formulário de categoria...');

    form.addEventListener('submit', (event) => {
        event.preventDefault(); // Impede o recarregamento da página
        console.log('Iniciando processamento dos dados da categoria...');
        processCategoryData(form);
    });

    console.log('✅ Event listener do formulário de categoria configurado com sucesso!');
}

/**
 * Prepara os dados do formulário para envio.
 * Usa FormData por causa do campo de arquivo.
 */
async function processCategoryData(form) {
    console.log('🔍 Preparando dados (FormData)...');

    // FormData é a maneira correta de capturar dados de um formulário que inclui arquivos.
    const formData = new FormData(form);

    // Renomeia os campos para corresponder ao que a API espera (ex: 'Name', 'Description', 'ImageFile')
    formData.set('Name', formData.get('categoryName'));
    formData.set('Description', formData.get('categoryDescription'));
    formData.set('ImageFile', formData.get('categoryImage'));
    
    // Remove os nomes antigos que não serão usados
    formData.delete('categoryName');
    formData.delete('categoryDescription');
    formData.delete('categoryImage');
    
    // Validação básica
    if (!formData.get('Name')) {
        alert('Por favor, preencha o nome da categoria.');
        return;
    }

    console.log('✅ Dados prontos para envio.');
    await sendCategoryData(formData, form);
}

/**
 * Envia os dados da categoria para a API.
 */
async function sendCategoryData(formData, form) {
    console.log('📡 Preparando dados da categoria para envio...');

    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) {
        alert('Você não está autenticado. Faça o login novamente.');
        return;
    }
    
    try {
        const response = await fetch('http://localhost:5087/api/categories', {
            method: 'POST',
            headers: {
                // Ao usar FormData, o único header que definimos é o de autorização.
                'Authorization': `Bearer ${accessToken}`
                // NÃO defina 'Content-Type'. O navegador faz isso automaticamente com o boundary correto.
            },
            body: formData, // Enviamos o objeto FormData diretamente no corpo.
        });

        if (response.status === 401) {
            alert('Sessão expirada. Faça login novamente.');
            return;
        }

        if (response.ok) {
            console.log('✅ Categoria salva com sucesso!');
            alert('Categoria cadastrada com sucesso!');
            form.reset();
        } else {
            const errorData = await response.json().catch(() => ({}));
            const errorMessage = errorData.message || 'Erro ao salvar a categoria. Verifique os dados.';
            console.error('❌ Erro da API:', errorMessage);
            alert(`Erro: ${errorMessage}`);
        }
    } catch (error) {
        console.error('❌ Erro na requisição:', error);
        alert('Falha na comunicação com o servidor. Verifique se a API está rodando.');
    }
}

// --- EXECUÇÃO PRINCIPAL ---
const formElement = document.querySelector('.category-form');
initializeCategoryForm(formElement);

// --- 
const fileInput = document.getElementById('categoryImage');
const fileNameDisplay = document.getElementById('fileName');
const imagePreview = document.getElementById('imagePreview');

fileInput.addEventListener('change', () => {
    const file = fileInput.files[0];
    if (file) {
    fileNameDisplay.textContent = file.name;

    const reader = new FileReader();
    reader.onload = function (e) {
        imagePreview.src = e.target.result;
        imagePreview.style.display = 'block';
    };
    reader.readAsDataURL(file);
    } else {
    fileNameDisplay.textContent = 'Nenhum arquivo selecionado';
    imagePreview.src = '';
    imagePreview.style.display = 'none';
    }
});