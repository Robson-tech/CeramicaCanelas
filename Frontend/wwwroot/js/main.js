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
    15: 'Pedreiro',
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
let isFormLoading = false; // Trava para evitar race conditions

// ... (o resto das suas variáveis globais)


// =======================================================
// FUNÇÃO PRINCIPAL DE CARREGAMENTO DE PÁGINAS (REFATORADA)
// =======================================================

/**
 * Carrega dinamicamente um formulário, seu CSS e seu script correspondente,
 * prevenindo carregamentos simultâneos para evitar race conditions.
 */
let isFormLoading = false; // Trava para evitar race conditions

async function loadForm(formName) {
    // 1. VERIFICA SE UM CARREGAMENTO JÁ ESTÁ EM ANDAMENTO
    if (isFormLoading) {
        console.warn(`🚦 AVISO: Tentativa de carregar '${formName}' enquanto outro formulário está em andamento. Ação ignorada.`);
        return;
    }

    // 2. ATIVA A TRAVA E INICIA O PROCESSO
    isFormLoading = true;
    console.log(`▶️ Iniciando carregamento do formulário: ${formName}. Trava ativada.`);

    const container = document.getElementById('form-container');
    if (!container) {
        console.error("❌ ERRO: Elemento 'form-container' não encontrado!");
        isFormLoading = false; // Libera a trava em caso de erro fatal
        return;
    }

    // Exibe a mensagem de carregamento
    container.innerHTML = '<h2>Carregando Formulário...</h2>';
    const welcomeMessage = document.getElementById('welcome-message');
    if (welcomeMessage) {
        welcomeMessage.style.display = 'none';
    }

    try {
        // 3. LIMPEZA DOS RECURSOS ANTERIORES
        document.getElementById('dynamic-form-script')?.remove();
        document.getElementById('dynamic-form-style')?.remove();

        const htmlUrl = `/forms/${formName}.html`;
        const cssUrl = `/css/${formName}.css`;
        const jsUrl = `/js/${formName}.js`;

        // 4. CARREGAMENTO PARALELO DE HTML E CSS
        const [htmlResponse, cssResponse] = await Promise.all([
            fetch(htmlUrl),
            fetch(cssUrl).catch(err => {
                console.warn(`⚠️ CSS em ${cssUrl} não encontrado, continuando sem ele.`);
                return null; // Falha no CSS não é fatal
            })
        ]);

        if (!htmlResponse.ok) {
            throw new Error(`Formulário ${formName}.html não encontrado (Status: ${htmlResponse.status}).`);
        }

        const htmlContent = await htmlResponse.text();

        // 5. INJEÇÃO DO CSS (SE EXISTIR)
        if (cssResponse && cssResponse.ok) {
            const cssContent = await cssResponse.text();
            const style = document.createElement('style');
            style.id = 'dynamic-form-style';
            style.textContent = cssContent;
            document.head.appendChild(style);
            console.log(`✅ CSS ${formName}.css carregado e injetado.`);
        }

        // 6. INJEÇÃO DO HTML
        container.innerHTML = htmlContent;
        console.log(`✅ HTML ${formName}.html injetado no container.`);

        // 7. CARREGAMENTO CONTROLADO DO SCRIPT USANDO PROMISE
        await new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.id = 'dynamic-form-script';
            script.src = jsUrl;

            script.onload = () => {
                console.log(`✅ Script ${formName}.js carregado.`);
                // Executa a função de inicialização do formulário, se existir
                if (typeof window.initDynamicForm === 'function') {
                    console.log(`🚀 Executando initDynamicForm() de ${formName}.js`);
                    // Idealmente, initDynamicForm também seria assíncrona e retornaria uma promise
                    // para ser aguardada aqui, mas para este caso, chamá-la já é suficiente.
                    window.initDynamicForm();
                } else {
                    console.warn(`⚠️ O script ${formName}.js não possui a função initDynamicForm().`);
                }
                resolve(); // Resolve a promise, indicando sucesso.
            };

            script.onerror = () => {
                console.error(`❌ Erro fatal ao carregar o script ${jsUrl}`);
                // Rejeita a promise, o que fará o bloco catch principal ser acionado.
                reject(new Error(`Falha ao carregar o script ${jsUrl}`));
            };

            document.body.appendChild(script);
        });

    } catch (error) {
        console.error('💥 Erro no processo de loadForm:', error);
        container.innerHTML = `<p style="color:red; text-align:center;">${error.message}</p>`;
    } finally {
        // 8. LIBERAÇÃO DA TRAVA
        // Este bloco será executado sempre, seja em caso de sucesso ou de erro,
        // garantindo que a aplicação não fique "travada".
        isFormLoading = false;
        console.log(`⏹️ Carregamento do formulário ${formName} finalizado. Trava liberada.`);
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

// Adicione esta função ao seu arquivo js/main.j, se ela não estiver lá
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