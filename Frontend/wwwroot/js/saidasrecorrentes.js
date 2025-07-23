console.log('Script js/relatorio-saidas.js DEFINIDO.');

// =======================================================
// INICIALIZAÇÃO
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de relatorio-saidas.js foi chamada.');
    initializeSearch();
}

function initializeSearch() {
    document.getElementById('searchButton')?.addEventListener('click', () => performSearch());
    document.getElementById('clearButton')?.addEventListener('click', clearFilters);
    
    if (typeof loadProductCategories === 'function') {
        loadProductCategories(document.getElementById('categoryId'), 'Todas as Categorias')
            .then(() => {
                performSearch();
            });
    } else {
        console.warn("Função 'loadProductCategories' não encontrada.");
        performSearch();
    }
}

function clearFilters() {
    document.getElementById('search').value = '';
    document.getElementById('categoryId').value = '';
    performSearch();
}

// =======================================================
// LÓGICA DE BUSCA E RENDERIZAÇÃO
// =======================================================
async function performSearch() {
    const loadingDiv = document.getElementById('loading');
    const resultsSection = document.getElementById('resultsSection');
    
    if(loadingDiv) loadingDiv.style.display = 'flex';
    if(resultsSection) resultsSection.style.display = 'none';

    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Não autenticado.");

        const params = new URLSearchParams();
        const search = document.getElementById('search')?.value;
        const categoryId = document.getElementById('categoryId')?.value;

        if (search) params.append('Search', search);
        if (categoryId) params.append('CategoryId', categoryId);

        const url = `${API_BASE_URL}/dashboard/reports/products/most-used?${params.toString()}`;
        console.log("📡 Buscando dados em:", url);

        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar dados (Status: ${response.status})`);

        const data = await response.json();
        
        // Valida se a resposta da API é um array, como esperado agora.
        if (!Array.isArray(data)) {
            throw new Error("A resposta da API não é um array de itens como o esperado.");
        }

        updateSummary(data);
        renderResultsTable(data);
        
        if(resultsSection) resultsSection.style.display = 'block';

    } catch (error) {
        if(typeof showErrorModal === 'function') {
            showErrorModal({ title: "Erro na Pesquisa", detail: error.message });
        } else {
            alert(`Erro na Pesquisa: ${error.message}`);
        }
    } finally {
        if(loadingDiv) loadingDiv.style.display = 'none';
    }
}

function updateSummary(items) {
    const summaryContainer = document.getElementById('resultsSummary');
    if (!summaryContainer) return;

    // Calcula os totais a partir do array de itens recebido
    const totalProducts = items ? items.length : 0;
    const totalExits = items ? items.reduce((sum, item) => sum + item.totalSaidas, 0) : 0;
    const totalStock = items ? items.reduce((sum, item) => sum + item.estoqueAtual, 0) : 0;
    const avgExits = totalProducts > 0 ? (totalExits / totalProducts).toFixed(1) : 0;
    
    summaryContainer.innerHTML = `
        <div class="summary-item">
            <div class="summary-value">${totalProducts}</div>
            <div class="summary-label">Produtos Analisados</div>
        </div>
        <div class="summary-item">
            <div class="summary-value">${totalExits}</div>
            <div class="summary-label">Total de Saídas</div>
        </div>
        <div class="summary-item">
            <div class="summary-value">${totalStock}</div>
            <div class="summary-label">Estoque Total</div>
        </div>
        <div class="summary-item">
            <div class="summary-value">${avgExits}</div>
            <div class="summary-label">Média de Saídas</div>
        </div>
    `;
}

function renderResultsTable(items) {
    const tableBody = document.getElementById('tableBody');
    const noResultsDiv = document.getElementById('noResults');
    if(!tableBody || !noResultsDiv) return;

    tableBody.innerHTML = '';

    if (!items || items.length === 0) {
        noResultsDiv.style.display = 'block';
        tableBody.parentElement.parentElement.style.display = 'none';
        return;
    }
    
    noResultsDiv.style.display = 'none';
    tableBody.parentElement.parentElement.style.display = 'block';

    items.forEach(item => {
        const formattedDate = item.ultimaRetirada ? new Date(item.ultimaRetirada).toLocaleString('pt-BR') : 'N/A';
        const row = tableBody.insertRow();
        row.innerHTML = `
            <td>${item.productName || 'N/A'}</td>
            <td>${item.category || 'N/A'}</td>
            <td>${item.totalSaidas}</td>
            <td>${formattedDate}</td>
            <td>${item.estoqueAtual}</td>
        `;
    });
}