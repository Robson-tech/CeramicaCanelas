// LOG 1: Confirma que o arquivo de script foi carregado e está sendo executado.
console.log('Script js/category.js (padrão similar ao de usuário) EXECUTANDO.');

// Defina a URL base da sua API aqui.


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
 * // ALTERAÇÃO 1: Agora cria um objeto JSON em vez de FormData.
 */
async function processCategoryData(form) {
    console.log('🔍 Preparando dados (JSON)...');

    // Pega os valores diretamente dos campos do formulário
    const categoryName = form.querySelector('[name="categoryName"]').value;
    const categoryDescription = form.querySelector('[name="categoryDescription"]').value;

    const categoryData = {
        Name: categoryName,
        Description: categoryDescription
    };
    
    if (!categoryData.Name) {
        alert('Por favor, preencha o nome da categoria.');
        return;
    }

    console.log('✅ Dados prontos para envio.');
    await sendCategoryData(categoryData, form); // Envia o objeto JSON
}

/**
 * Envia os dados da categoria para a API.
 */
async function sendCategoryData(categoryData, form) { // ALTERAÇÃO 2: O parâmetro agora é 'categoryData'
    console.log('📡 Preparando dados da categoria para envio...');

    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) {
        alert('Você não está autenticado. Faça o login novamente.');
        return;
    }
    
    try {
        const url = `${API_BASE_URL}/categories`;

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                // ALTERAÇÃO 3: Header Content-Type é ESSENCIAL para enviar JSON.
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${accessToken}`
            },
            // ALTERAÇÃO 4: O corpo da requisição agora é o objeto de dados convertido para uma string JSON.
            body: JSON.stringify(categoryData),
        });

        if (response.status === 401) {
            alert('Sessão expirada. Faça login novamente.');
            return;
        }

        if (response.ok) {
            console.log('✅ Categoria salva com sucesso!');
            alert('Categoria cadastrada com sucesso!');
            form.reset();
            // Dispara o evento 'change' para limpar o preview da imagem
            const imageInput = document.getElementById('categoryImage');
            if (imageInput) {
                imageInput.dispatchEvent(new Event('change'));
            }
        } else {
            const errorData = await response.json().catch(() => ({}));
            const errorMessage = errorData.message || errorData.title || 'Erro ao salvar a categoria. Verifique os dados.';
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
if (formElement) {
    initializeCategoryForm(formElement);
}

// --- LÓGICA DO PREVIEW DE IMAGEM ---
// (Esta parte pode ser mantida como está, pois ela afeta apenas a interface e não o envio dos dados)
const fileInput = document.getElementById('categoryImage');
const fileNameDisplay = document.getElementById('fileName');
const imagePreview = document.getElementById('imagePreview');

if (fileInput) {
    fileInput.addEventListener('change', () => {
        const file = fileInput.files[0];
        if (file) {
            if(fileNameDisplay) fileNameDisplay.textContent = file.name;
    
            const reader = new FileReader();
            reader.onload = function (e) {
                if(imagePreview) {
                    imagePreview.src = e.target.result;
                    imagePreview.style.display = 'block';
                }
            };
            reader.readAsDataURL(file);
        } else {
            if(fileNameDisplay) fileNameDisplay.textContent = 'Nenhum arquivo selecionado';
            if(imagePreview) {
                imagePreview.src = '';
                imagePreview.style.display = 'none';
            }
        }
    });
}