console.log('Script js/relatorio-saidas.js DEFINIDO.');



// =======================================================
// INICIALIZAÇÃO
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de relatorio-saidas.js foi chamada.');
    initializeFilters();
    // Usa a função genérica para carregar as categorias de lançamento financeiro
    if (typeof loadLaunchCategories === 'function') {
        loadLaunchCategories(document.getElementById('category-filter'), 'Todas as Categorias')
            .then(() => {
                fetchReportData(1);
            });
    } else {
        console.warn("Função 'loadLaunchCategories' não encontrada. O filtro de categoria não será populado.");
        fetchReportData(1);
    }
}

function initializeFilters() {
    document.getElementById('searchButton')?.addEventListener('click', () => fetchReportData(1));
    document.getElementById('clearButton')?.addEventListener('click', clearFilters);
}

function clearFilters() {
    document.getElementById('search-input').value = '';
    document.getElementById('category-filter').value = '';
    document.getElementById('start-date').value = '';
    document.getElementById('end-date').value = '';
    fetchReportData(1);
}

// =======================================================
// LÓGICA DE BUSCA E RENDERIZAÇÃO
// =======================================================
async function fetchReportData(page = 1) {
    currentPage = page;
    const loadingDiv = document.getElementById('loading');
    const resultsSection = document.getElementById('resultsSection');
    
    if(loadingDiv) loadingDiv.style.display = 'flex';
    if(resultsSection) resultsSection.style.display = 'none';

    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Não autenticado.");

        const params = new URLSearchParams({ Page: currentPage, PageSize: 10 });
        const search = document.getElementById('search-input')?.value;
        const categoryId = document.getElementById('category-filter')?.value;
        const startDate = document.getElementById('start-date')?.value;
        const endDate = document.getElementById('end-date')?.value;

        if (search) params.append('Search', search);
        if (categoryId) params.append('CategoryId', categoryId);
        if (startDate) params.append('StartDate', new Date(startDate).toISOString());
        if (endDate) params.append('EndDate', new Date(endDate).toISOString());

        const url = `${API_BASE_URL}/financial/dashboard-financial/balance-expense?${params.toString()}`;
        console.log("📡 Buscando dados em:", url);

        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar dados (Status: ${response.status})`);

        const data = await response.json();
        
        renderReportTable(data.items);
        renderPagination(data);
        
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

function renderReportTable(items) {
    const tableBody = document.getElementById('report-table-body');
    const noResultsDiv = document.getElementById('noResults');
    if(!tableBody || !noResultsDiv) return;

    tableBody.innerHTML = '';

    if (!items || items.length === 0) {
        noResultsDiv.style.display = 'block';
        return;
    }
    
    noResultsDiv.style.display = 'none';

    items.forEach(item => {
        const formattedDate = new Date(item.launchDate).toLocaleDateString('pt-BR');
        const formattedAmount = (item.amount || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        const statusText = (typeof statusMap !== 'undefined' && statusMap[item.status]) ? statusMap[item.status] : 'N/A';
        
        const row = tableBody.insertRow();
        row.innerHTML = `
            <td>${item.description || 'N/A'}</td>
            <td>${item.categoryName || 'N/A'}</td>
            <td>${formattedDate}</td>
            <td>${statusText}</td>
            <td class="expense">${formattedAmount}</td>
        `;
    });
}

function renderPagination(paginationData) {
    const controlsContainer = document.getElementById('pagination-controls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    
    if (!paginationData || !paginationData.totalPages || paginationData.totalPages <= 1) return;
    
    const { page, totalPages } = paginationData;
    
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = page <= 1;
    prevButton.onclick = () => fetchReportData(page - 1);
    
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${page} de ${totalPages}`;
    pageInfo.className = 'pagination-info';
    
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = page >= totalPages;
    nextButton.onclick = () => fetchReportData(page + 1);
    
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}