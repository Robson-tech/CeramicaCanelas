console.log('Script js/estoque.js DEFINIDO.');

// =======================================================
// INICIALIZAÇÃO
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de estoque.js foi chamada.');
    initializeSearch();
}

function initializeSearch() {
    document.getElementById('searchButton')?.addEventListener('click', performSearch);
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

        const url = `${API_BASE_URL}/dashboard/reports/products/stock-outof?${params.toString()}`;
        console.log("📡 Buscando dados em:", url);

        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar dados (Status: ${response.status})`);

        const items = await response.json();
        
        updateSummary(items);
        renderResultsTable(items);
        
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

function updateSummary(items = []) {
    const summaryContainer = document.getElementById('resultsSummary');
    if (!summaryContainer) return;

    const totalProducts = items.length;
    let outOfStock = 0;
    let lowStock = 0;

    items.forEach(item => {
        if (item.stockCurrent <= 0) {
            outOfStock++;
        } else if (item.stockCurrent < item.stockMinimum) {
            lowStock++;
        }
    });

    const normalStock = totalProducts - outOfStock - lowStock;

    summaryContainer.innerHTML = `
        <div class="summary-item">
            <div class="summary-value">${totalProducts}</div>
            <div class="summary-label">Total de Produtos</div>
        </div>
        <div class="summary-item">
            <div class="summary-value">${outOfStock}</div>
            <div class="summary-label">Em Falta</div>
        </div>
        <div class="summary-item">
            <div class="summary-value">${lowStock}</div>
            <div class="summary-label">Estoque Baixo</div>
        </div>
        <div class="summary-item">
            <div class="summary-value">${normalStock}</div>
            <div class="summary-label">Estoque Normal</div>
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
        if(tableBody.parentElement.parentElement) tableBody.parentElement.parentElement.style.display = 'none';
        return;
    }
    
    noResultsDiv.style.display = 'none';
    if(tableBody.parentElement.parentElement) tableBody.parentElement.parentElement.style.display = 'block';

    items.forEach(item => {
        const statusInfo = getStatusInfo(item.stockCurrent, item.stockMinimum);
        const formattedDate = item.lastExit ? new Date(item.lastExit).toLocaleDateString('pt-BR') : 'N/A';
        
        const row = tableBody.insertRow();
        row.innerHTML = `
            <td>${item.productName || 'N/A'}</td>
            <td>${item.category || 'N/A'}</td>
            <td>${item.stockCurrent}</td>
            <td>${item.stockMinimum}</td>
            <td>${formattedDate}</td>
            <td><span class="status-badge ${statusInfo.className}">${statusInfo.text}</span></td>
        `;
    });
}

function getStatusInfo(current, min) {
    if (current <= 0) {
        return { text: "Em Falta", className: "status-danger" };
    }
    if (current < min) {
        return { text: "Estoque Baixo", className: "status-warning" };
    }
    return { text: "Normal", className: "status-success" };
}