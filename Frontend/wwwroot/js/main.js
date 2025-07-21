// =======================================================
// VARIÁVEIS E CONSTANTES GLOBAIS DA APLICAÇÃO
// =======================================================
// const API_BASE_URL = 'https://api.ceramicacanelas.shop/api';
const API_BASE_URL = 'http://localhost:5087/api';

// Objetos para guardar o estado original da linha durante a edição
const originalRowHTML_Product = {};
const originalRowHTML_Employee = {};
const originalRowHTML_Category = {};
const originalRowHTML_Supplier = {};
// Utiliza as variáveis globais de main.js

let currentEmployeePage = 1;
const originalEntryRowHTML = {}; 
// Variável para controlar a paginação da tabela atual
let currentTablePage = 1;
// Este script utiliza as variáveis globais definidas em main.js
 // Página atual da MODAL de busca de produtos
// Mapa de cargos para ser usado na tela de funcionários
const positionMap = {
    0: 'Enfornador', 1: 'Desenfornador', 2: 'Soldador', 3: 'Marombeiro',
    4: 'Operador de Pá Carregadeira', 5: 'Motorista', 6: 'Queimador',
    7: 'Conferente', 8: 'Caixa', 9: 'Auxiliar Administrativo',
    10: 'Auxiliar de Limpeza', 11: 'Dono', 12: 'Gerente', 13: 'Auxiliar de Estoque'
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
function loadForm(formName) {
    console.log(`▶️ Iniciando carregamento do formulário: ${formName}`);
    
    const container = document.getElementById('form-container');
    if (!container) {
        console.error("❌ ERRO: Elemento 'form-container' não encontrado!");
        return;
    }
    
    const welcomeMessage = document.getElementById('welcome-message');
    if (welcomeMessage) {
        welcomeMessage.style.display = 'none';
    }

    container.innerHTML = '<h2>Carregando Formulário...</h2>';
    const oldScript = document.getElementById('dynamic-form-script');
    if (oldScript) {
        oldScript.remove();
    }

    fetch(`/forms/${formName}.html`)
        .then(response => {
            if (!response.ok) throw new Error(`Formulário ${formName}.html não encontrado.`);
            return response.text();
        })
        .then(html => {
            container.innerHTML = html;
            
            const script = document.createElement('script');
            script.id = 'dynamic-form-script';
            script.src = `/js/${formName}.js`;
            
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
                console.error(`❌ Erro fatal ao carregar o script ${formName}.js`);
            };
            
            document.body.appendChild(script);
        })
        .catch(error => {
            console.error('💥 Erro no processo de loadForm:', error);
            container.innerHTML = `<p style="color:red; text-align:center;">${error.message}</p>`;
        });
}
function showErrorModal(errorData) {
    const modal = document.getElementById('errorModal');
    if (!modal) {
        // Alternativa caso a modal não exista no HTML
        alert((errorData.title || "Erro") + "\n\n" + (errorData.detail || "Ocorreu um erro."));
        return;
    }

    const titleElement = document.getElementById('errorModalTitle');
    const detailElement = document.getElementById('errorModalDetail');
    const closeBtn = document.getElementById('errorModalCloseBtn');

    titleElement.textContent = errorData.title || 'Erro Inesperado';
    detailElement.textContent = errorData.detail || 'Não foram fornecidos mais detalhes pelo servidor.';
    modal.style.display = 'block';

    closeBtn.onclick = () => modal.style.display = 'none';
    window.onclick = (event) => {
        if (event.target == modal) modal.style.display = 'none';
    };
}
