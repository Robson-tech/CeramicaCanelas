// =======================================================
// VARIÁVEIS E CONSTANTES GLOBAIS DA APLICAÇÃO
// =======================================================
const API_BASE_URL = 'https://api.ceramicacanelas.shop/api';
// const API_BASE_URL = 'http://localhost:5087/api';
let historyItemsCache = []; // Cache para os dados do histórico
// Objetos para guardar o estado original da linha durante a edição
const originalRowHTML_Product = {};
const originalRowHTML_Employee = {};
const originalRowHTML_Category = {};
const originalRowHTML_Supplier = {};
// Utiliza as variáveis globais de main.js
const originalHistoryRowHTML = {}; // Objeto para a edição na tabela de histórico
let currentPage = 1;
let currentEmployeePage = 1;
const originalEntryRowHTML = {}; 
// Variável para controlar a paginação da tabela atual
let currentTablePage = 1;
// Este script utiliza as variáveis globais definidas em main.js
 // Página atual da MODAL de busca de produtos
// Mapa de cargos para ser usado na tela de funcionários
const positionMap = {
    0: 'Enfornador',
    1: 'Desenfornador',
    2: 'Soldador',
    3: 'Marombeiro',
    4: 'Operador de Pá Carregadeira',
    5: 'Motorista',
    6: 'Queimador',
    7: 'Conferente',
    8: 'Caixa',
    9: 'Auxiliar Administrativo',
    10: 'Auxiliar de Limpeza',
    11: 'Dono',
    12: 'Gerente',
    13: 'Auxiliar de Estoque',
    14: 'Prestador de Serviços',
};

// NOVO: Mapa para traduzir os números das funções para texto
const userRolesMap = {
    0: 'Admin',
    1: 'Customer',
    2: 'Viewer'
};

// Função utilitária global
const getPositionName = (positionId) => positionMap[positionId] || 'Desconhecido';

// Defina a URL base da sua API aqui.
let currentSupplierPage = 1;

let currentEntryPage = 1;
let currentModalPage = 1;
let currentSupplierModalPage = 1;

let allEmployees = [];

let currentHistoryPage = 1;
let currentProductModalPage = 1;
// =======================================================
// FUNÇÃO PRINCIPAL DE CARREGAMENTO DE PÁGINAS
// =======================================================

/**
 * Carrega dinamicamente um formulário e seu script correspondente.
 */
/**
 * Carrega dinamicamente um formulário, seu CSS e seu script correspondente.
 * Garante que o CSS seja aplicado antes do HTML ser exibido para evitar FOUC.
 */
async function loadForm(formName) {
    console.log(`▶️ Iniciando carregamento completo do formulário: ${formName}`);

    const container = document.getElementById('form-container');
    if (!container) {
        console.error("❌ ERRO: Elemento 'form-container' não encontrado!");
        return;
    }

    // Exibe a mensagem de carregamento e oculta a de boas-vindas
    container.innerHTML = '<h2>Carregando Formulário...</h2>';
    const welcomeMessage = document.getElementById('welcome-message');
    if (welcomeMessage) {
        welcomeMessage.style.display = 'none';
    }

    // 1. Limpa o script e o estilo (CSS) dinâmicos da carga anterior
    document.getElementById('dynamic-form-script')?.remove();
    document.getElementById('dynamic-form-style')?.remove();

    // 2. Define as URLs para os arquivos HTML, CSS e JS
    const htmlUrl = `/forms/${formName}.html`;
    const cssUrl = `/css/${formName}.css`; // Assumindo que o CSS fica em /css/
    const jsUrl = `/js/${formName}.js`;

    try {
        // 3. Tenta carregar HTML e CSS em paralelo para otimizar
        const [htmlResponse, cssResponse] = await Promise.all([
            fetch(htmlUrl),
            fetch(cssUrl).catch(err => {
                // Permite que a função continue mesmo se o CSS não for encontrado (404)
                console.warn(`⚠️ CSS em ${cssUrl} não encontrado, carregando sem ele.`);
                return null; // Retorna null para indicar falha opcional
            })
        ]);

        if (!htmlResponse.ok) {
            throw new Error(`Formulário ${formName}.html não encontrado (Status: ${htmlResponse.status}).`);
        }

        const htmlContent = await htmlResponse.text();

        // 4. Processa e injeta o CSS (se foi carregado com sucesso)
        if (cssResponse && cssResponse.ok) {
            const cssContent = await cssResponse.text();
            const style = document.createElement('style');
            style.id = 'dynamic-form-style';
            style.textContent = cssContent;
            document.head.appendChild(style);
            console.log(`✅ CSS ${formName}.css carregado e injetado.`);
        }

        // 5. Injeta o HTML no container (agora que o CSS já está aplicado)
        container.innerHTML = htmlContent;

        // 6. Carrega o script JavaScript associado
        const script = document.createElement('script');
        script.id = 'dynamic-form-script';
        script.src = jsUrl;

        script.onload = () => {
            console.log(`✅ Script ${formName}.js carregado com sucesso.`);
            if (typeof window.initDynamicForm === 'function') {
                console.log(`🚀 Executando initDynamicForm() de ${formName}.js`);
                window.initDynamicForm();
            } else {
                console.warn(`⚠️ AVISO: O script ${formName}.js não possui a função initDynamicForm().`);
            }
        };

        script.onerror = () => {
            console.error(`❌ Erro fatal ao carregar o script ${jsUrl}`);
            // Opcional: Reverter para uma mensagem de erro se o script for crucial
        };

        document.body.appendChild(script);

    } catch (error) {
        console.error('💥 Erro no processo de loadForm:', error);
        container.innerHTML = `<p style="color:red; text-align:center;">${error.message}</p>`;
    }
}
function showErrorModal(errorData) {
    // const modal = document.getElementById('errorModal');
    // if (!modal) {
    //     // Alternativa caso a modal não exista no HTML
    //     alert((errorData.title || "Erro") + "\n\n" + (errorData.detail || "Ocorreu um erro."));
    //     return;
    // }

    // const titleElement = document.getElementById('errorModalTitle');
    // const detailElement = document.getElementById('errorModalDetail');
    // const closeBtn = document.getElementById('errorModalCloseBtn');

    // titleElement.textContent = errorData.title || 'Erro Inesperado';
    // detailElement.textContent = errorData.detail || 'Não foram fornecidos mais detalhes pelo servidor.';
    // modal.style.display = 'block';

    // closeBtn.onclick = () => modal.style.display = 'none';
    // window.onclick = (event) => {
    //     if (event.target == modal) modal.style.display = 'none';
    // };
}

// Adicione esta função ao seu arquivo js/main.js, se ela não estiver lá
async function loadProductCategories(selectElement, defaultOptionText = 'Selecione uma categoria') { 
    if (!selectElement) return; 
    
    try { 
        const accessToken = localStorage.getItem('accessToken'); 
        const response = await fetch(`${API_BASE_URL}/categories`, { headers: { 'Authorization': `Bearer ${accessToken}` } }); 
        
        // --- CORREÇÃO APLICADA AQUI ---
        // Verifica primeiro se a requisição falhou
        if (!response.ok) {
            // Se o erro for um 400 (Bad Request), tentamos ler a mensagem
            if (response.status === 400) {
                const errorData = await response.json();
                // Verificamos se é a mensagem específica de "sem categorias"
                if (errorData && errorData.message === "Não há categórias cadastradas.") {
                    selectElement.innerHTML = `<option value="">Nenhuma categoria cadastrada</option>`;
                    selectElement.disabled = true;
                    return; // Encerra a função aqui, pois já tratamos o caso
                }
            }
            // Para todos os outros erros, lança uma exceção
            throw new Error(`Falha ao carregar categorias (Status: ${response.status})`);
        }
        
        const categories = await response.json(); 
        
        // Esta parte continua a mesma para o caso de sucesso com uma lista vazia
        if (!categories || categories.length === 0) {
            selectElement.innerHTML = `<option value="">Nenhuma categoria cadastrada</option>`;
            selectElement.disabled = true;
        } else {
            // Se houver categorias, preenche o select normalmente
            selectElement.disabled = false;
            selectElement.innerHTML = `<option value="">${defaultOptionText}</option>`; 
            categories.forEach(category => { 
                const option = new Option(category.name, category.id); 
                selectElement.appendChild(option); 
            }); 
        }
        // --- FIM DA CORREÇÃO ---

    } catch (error) { 
        console.error('Erro ao carregar categorias:', error); 
        selectElement.innerHTML = '<option value="">Erro ao carregar</option>'; 
        // Não relançamos o erro para não quebrar a inicialização de outras partes da página
    } 
}