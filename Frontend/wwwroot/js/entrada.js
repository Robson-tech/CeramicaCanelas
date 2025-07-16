console.log('Script js/product-entry.js DEFINIDO (versão GET Simples).');

const API_BASE_URL = 'http://localhost:5087/api';

// Variáveis de estado (não utilizadas nesta versão, mas mantidas para estrutura)
let currentPage = 1;
let paginationData = {}; 

function initDynamicForm() {
    console.log('▶️ initDynamicForm() de product-entry.js foi chamada.');
    const formElement = document.querySelector('#productForm');
    initializeFormListeners(formElement);
}

function initializeFormListeners(form) {
    if (!form) return;
    const modal = document.getElementById('productSearchModal');
    const openModalBtn = document.getElementById('openProductModalBtn');
    const closeModalBtn = modal.querySelector('.modal-close-btn');
    
    openModalBtn.addEventListener('click', () => {
        modal.style.display = 'block';
        fetchAndRenderProductsPage(); 
    });
    closeModalBtn.addEventListener('click', () => modal.style.display = 'none');
    window.addEventListener('click', (event) => {
        if (event.target == modal) modal.style.display = 'none';
    });
    initializeMainFormSubmit(form);
    initializeProductSelectionListener(modal);
}

/**
 * Função central para buscar e renderizar uma página de produtos.
 * VERSÃO DE TESTE: Fazendo uma chamada GET simples, sem nenhum parâmetro na URL.
 */
async function fetchAndRenderProductsPage() {
    const resultsContainer = document.getElementById('modalResultsContainer');
    resultsContainer.innerHTML = '<p>Buscando...</p>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Token de acesso não encontrado.");

        const url = `${API_BASE_URL}/products/paged`;
        console.log("📡 [TESTE] Enviando requisição GET simples para:", url);

        const response = await fetch(url, {
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });

        if (!response.ok) {
            throw new Error(`Falha na requisição: ${response.status} ${response.statusText}`);
        }
        
        const data = await response.json();
        
        // Como não podemos controlar a paginação, vamos esconder os botões.
        const paginationControls = document.getElementById('modalPaginationControls');
        if (paginationControls) {
            paginationControls.innerHTML = '';
        }
        
        renderModalResults(data.items, resultsContainer);

    } catch (error) {
        resultsContainer.innerHTML = `<p style="color:red;">${error.message}</p>`;
        const paginationControls = document.getElementById('modalPaginationControls');
        if (paginationControls) {
            paginationControls.innerHTML = '';
        }
        console.error("❌ Erro em fetchAndRenderProductsPage:", error);
    }
}

// O restante do código não precisa de alteração
function renderModalResults(products, container) {
    if (!products || products.length === 0) {
        container.innerHTML = '<p>Nenhum produto encontrado.</p>';
        return;
    }
    const table = document.createElement('table');
    table.className = 'results-table';
    table.innerHTML = `
        <thead><tr><th>Nome</th><th>Código</th><th>Ação</th></tr></thead>
        <tbody>
            ${products.map(product => `
                <tr>
                    <td>${product.name}</td>
                    <td>${product.code || 'N/A'}</td>
                    <td><button type="button" class="select-product-btn" data-id="${product.id}" data-name="${product.name}">Selecionar</button></td>
                </tr>
            `).join('')}
        </tbody>`;
    container.innerHTML = '';
    container.appendChild(table);
}

function initializeProductSelectionListener(modal) {
    const resultsContainer = modal.querySelector('#modalResultsContainer');
    resultsContainer.addEventListener('click', (event) => {
        if (event.target.classList.contains('select-product-btn')) {
            const productId = event.target.dataset.id;
            const productName = event.target.dataset.name;
            document.getElementById('selectedProductName').textContent = productName;
            document.getElementById('productUuid').value = productId;
            modal.style.display = 'none';
        }
    });
}

function initializeMainFormSubmit(form) {
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const productId = form.productUuid.value;
        const quantity = parseInt(form.quantity.value, 10);
        const unitPrice = parseFloat(form.unitPrice.value.replace(',', '.'), 10);
        if (!productId) { alert('Por favor, busque e selecione um produto.'); return; }
        const payload = { productId, quantity, unitPrice };
        const accessToken = localStorage.getItem('accessToken');
        try {
            const response = await fetch(`${API_BASE_URL}/products-entry`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${accessToken}`},
                body: JSON.stringify(payload)
            });
            if (response.ok) {
                alert('Entrada registrada com sucesso!');
                form.reset();
                document.getElementById('selectedProductName').textContent = 'Nenhum produto selecionado';
            } else {
                const errorData = await response.json();
                alert(`Erro: ${errorData.message || 'Não foi possível registrar a entrada.'}`);
            }
        } catch (error) {
            alert('Falha na comunicação com a API.');
        }
    });
}