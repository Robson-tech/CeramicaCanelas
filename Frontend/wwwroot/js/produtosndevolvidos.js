console.log('Script js/retiradas.js DEFINIDO.');

// =======================================================
// INICIALIZAÇÃO
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de retiradas.js foi chamada.');
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
    document.getElementById('employeeName').value = '';
    document.getElementById('startDate').value = '';
    document.getElementById('endDate').value = '';
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

        // A API de retiradas não usa paginação, então montamos a URL sem Page e PageSize
        const params = new URLSearchParams();
        const search = document.getElementById('search')?.value;
        const categoryId = document.getElementById('categoryId')?.value;
        const employeeName = document.getElementById('employeeName')?.value;
        const startDate = document.getElementById('startDate')?.value;
        const endDate = document.getElementById('endDate')?.value;

        if (search) params.append('Search', search);
        if (categoryId) params.append('CategoryId', categoryId);
        if (employeeName) params.append('EmployeeName', employeeName);
        if (startDate) params.append('StartDate', new Date(startDate).toISOString());
        if (endDate) params.append('EndDate', new Date(endDate).toISOString());

        const url = `${API_BASE_URL}/dashboard/reports/products/unreturned-products?${params.toString()}`;
        console.log("📡 Buscando dados em:", url);

        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar dados (Status: ${response.status})`);

        // --- CORREÇÃO APLICADA AQUI ---
        // A API retorna um array direto, então 'items' é o próprio resultado.
        const items = await response.json();
        
        console.log("📦 Resposta da API de Retiradas:", items);

        // Como não há dados de resumo ou paginação, desativamos essas funções
        // updateSummary(data); 
        renderResultsTable(items);
        // renderPagination(data);
        
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

function updateSummary(data) {
    // Esta função fica desativada por enquanto, pois a API não retorna os dados de resumo.
    const summaryContainer = document.getElementById('resultsSummary');
    if (summaryContainer) summaryContainer.innerHTML = '';
}

function renderResultsTable(items) {
    const tableBody = document.getElementById('tableBody');
    const noResultsDiv = document.getElementById('noResults');
    if(!tableBody || !noResultsDiv) return;

    tableBody.innerHTML = '';

    if (!items || !Array.isArray(items) || items.length === 0) {
        noResultsDiv.style.display = 'block';
        if(tableBody.parentElement.parentElement) tableBody.parentElement.parentElement.style.display = 'none';
        return;
    }
    
    noResultsDiv.style.display = 'none';
    if(tableBody.parentElement.parentElement) tableBody.parentElement.parentElement.style.display = 'block';

    items.forEach(item => {
        const statusClass = item.quantityPendente > 0 ? 'status-pending' : 'status-returned';
        const statusText = item.quantityPendente > 0 ? 'Pendente' : 'Finalizado';
        const formattedDate = new Date(item.dataRetirada).toLocaleDateString('pt-BR');
        
        const row = tableBody.insertRow();
        row.innerHTML = `
            <td>${item.productName || 'N/A'}</td>
            <td>${item.employeeName || 'N/A'}</td>
            <td>${item.quantityRetirada}</td>
            <td>${item.quantityDevolvida}</td>
            <td>${item.quantityPendente}</td>
            <td>${formattedDate}</td>
            <td><span class="status-badge ${statusClass}">${statusText}</span></td>
        `;
    });
}

function renderPagination(paginationData) {
    // Esta função fica desativada por enquanto.
    const controlsContainer = document.getElementById('pagination-controls');
    if (controlsContainer) controlsContainer.innerHTML = '';
}