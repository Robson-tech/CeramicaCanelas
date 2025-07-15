console.log('Script js/product.js EXECUTANDO.');

/**
 * Busca as categorias da API e preenche o campo <select> no formulário.
 */
async function loadProductCategories() {
    console.log('Buscando categorias para o formulário de produtos...');
    const categorySelect = document.querySelector('select[name="CategoryId"]');
    const accessToken = localStorage.getItem('accessToken');

    if (!categorySelect) return; // Se o select não existir, não faz nada.

    if (!accessToken) {
        categorySelect.innerHTML = '<option value="">Falha ao autenticar</option>';
        return;
    }

    try {
        const response = await fetch('http://localhost:5087/api/categories', {
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });

        if (!response.ok) {
            throw new Error('Não foi possível carregar as categorias.');
        }

        const categories = await response.json();
        
        // Limpa o select e adiciona as opções recebidas da API
        categorySelect.innerHTML = '<option value="">Selecione uma categoria</option>'; 
        categories.forEach(category => {
            const option = document.createElement('option');
            option.value = category.id; // O valor é o UUID da categoria
            option.textContent = category.name; // O texto é o nome
            categorySelect.appendChild(option);
        });
        console.log('Categorias carregadas com sucesso no select.');

    } catch (error) {
        console.error('Erro ao carregar categorias:', error);
        categorySelect.innerHTML = '<option value="">Erro ao carregar</option>';
    }
}

/**
 * Função principal que inicializa o formulário de produto.
 */
function initializeProductForm(form) {
    if (!form) {
        console.error('FALHA CRÍTICA: Elemento <form class="product-form"> não encontrado.');
        return;
    }

    console.log('🚀 Inicializando formulário de produto...');
    form.addEventListener('submit', (event) => {
        event.preventDefault();
        processProductData(form);
    });
    console.log('✅ Event listener do formulário de produto configurado com sucesso!');
}

/**
 * Prepara os dados do formulário para envio usando FormData.
 */
async function processProductData(form) {
    console.log('🔍 Preparando dados do produto (FormData)...');
    const formData = new FormData(form);

    // Validação
    if (!formData.get('Code') || !formData.get('Name') || !formData.get('CategoryId')) {
        alert('Por favor, preencha os campos obrigatórios: Código, Nome e Categoria.');
        return;
    }

    console.log('✅ Dados do produto prontos para envio.');
    await sendProductData(formData, form);
}

/**
 * Envia os dados do novo produto para a API.
 */
async function sendProductData(formData, form) {
    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) {
        alert('Você não está autenticado. Faça o login novamente.');
        return;
    }

    const API_URL = 'http://localhost:5087/api/products';
    console.log('📡 Enviando dados do produto para a API...');

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData,
        });

        if (response.status === 401) {
            alert('Sessão expirada. Faça login novamente.');
            return;
        }

        if (response.ok) {
            alert('Produto cadastrado com sucesso!');
            form.reset();
        } else {
            const errorData = await response.json().catch(() => ({}));
            const errorMessage = errorData.title || errorData.message || 'Erro ao salvar o produto.';
            console.error('❌ Erro da API:', errorMessage, errorData);
            alert(`Erro: ${errorMessage}`);
        }
    } catch (error) {
        console.error('❌ Erro na requisição:', error);
        alert('Falha na comunicação com o servidor.');
    }
}

// --- EXECUÇÃO PRINCIPAL ---
const formElement = document.querySelector('.product-form');
initializeProductForm(formElement); // Anexa o evento de submit ao formulário
loadProductCategories(); // Busca e preenche as categorias assim que o script é carregado