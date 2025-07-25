console.log('Script js/categoriadash.js DEFINIDO.');

// =======================================================
// INICIALIZAÇÃO
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de categoriadash.js foi chamada.');
    initializeSearch();
}

function initializeSearch() {
    const searchButton = document.getElementById('searchButton');
    if (searchButton) {
        searchButton.onclick = () => performSearch(1);
    }
    
    if (typeof loadProductCategories === 'function') {
        loadProductCategories(document.getElementById('categoryId'), 'Todas as Categorias');
    } else {
        console.warn("Função 'loadProductCategories' não foi encontrada.");
    }
    
    performSearch(1);
}

// =======================================================
// LÓGICA DE BUSCA E RENDERIZAÇÃO
// =======================================================
async function performSearch(page = 1) {
    const loadingDiv = document.getElementById('loading');
    const resultsSection = document.getElementById('resultsSection');
    const tableBody = document.getElementById('tableBody');

    if(loadingDiv) loadingDiv.style.display = 'flex';
    if(resultsSection) resultsSection.style.display = 'none';
    if (tableBody) tableBody.innerHTML = '';

    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Não autenticado.");

        const categoryId = document.getElementById('categoryId')?.value;
        const year = document.getElementById('year')?.value;
        const pageSize = 10;

        const params = new URLSearchParams({
            Page: page,
            PageSize: pageSize
        });
        if (categoryId) params.append('CategoryId', categoryId);
        if (year) params.append('Year', year);
        
        const url = `${API_BASE_URL}/dashboard/financial/monthly-cost-category?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        
        // --- CORREÇÃO APLICADA AQUI ---
        // Se a resposta for um erro, mas não queremos mostrar a modal
        if (!response.ok) {
            // Se for um erro 500 (ou qualquer outro), tratamos como "sem dados" e saímos da função
            console.warn(`A API retornou um erro ${response.status}. Exibindo a tela como vazia.`);
            updateSummary({}); // Chama com objeto vazio para limpar os cards
            renderResultsTable([]); // Chama com array vazio para mostrar "Nenhum resultado"
            renderPagination({}); // Chama com objeto vazio para limpar a paginação
            if(resultsSection) resultsSection.style.display = 'block'; // Mostra a seção de resultados vazia
            return; // Encerra a execução aqui
        }

        const data = await response.json();
        const paginatedData = data.pagedData; 
        
        if (!paginatedData || !paginatedData.items) {
            throw new Error("A resposta da API não tem o formato esperado.");
        }

        updateSummary(data);
        renderResultsTable(paginatedData.items);
        renderPagination(paginatedData);
        
        if(resultsSection) resultsSection.style.display = 'block';

    } catch (error) {
        // O catch agora só vai pegar erros de conexão ou falhas inesperadas no código
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
    const totalItems = data.pagedData?.totalItems || 0;
    const grandTotalCost = data.totalCostOverall || 0;
    const averageCost = data.averageCostPerRecord || 0;

    const summaryContainer = document.getElementById('resultsSummary');
    summaryContainer.innerHTML = `
        <div class="summary-item">
            <div class="summary-value">${totalItems}</div>
            <div class="summary-label">Total de Registros</div>
        </div>
        <div class="summary-item">
            <div class="summary-value">${grandTotalCost.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</div>
            <div class="summary-label">Custo Total</div>
        </div>
        <div class="summary-item">
            <div class="summary-value">${averageCost.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</div>
            <div class="summary-label">Custo Médio por Registro</div>
        </div>
    `;
}

function renderResultsTable(items) {
    const tableBody = document.getElementById('tableBody');
    const noResultsDiv = document.getElementById('noResults');
    tableBody.innerHTML = '';

    if (!items || items.length === 0) {
        if(noResultsDiv) noResultsDiv.style.display = 'block';
        return;
    }
    
    if(noResultsDiv) noResultsDiv.style.display = 'none';
    
    const monthNames = ["Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];

    items.forEach(item => {
        const row = tableBody.insertRow();
        row.innerHTML = `
            <td>${item.categoryName || 'N/A'}</td>
            <td>${item.year}</td>
            <td>${monthNames[item.month - 1] || item.month}</td>
            <td class="cost-cell">${(item.totalCost || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</td>
        `;
    });
}

function renderPagination(paginationData) {
    const controlsContainer = document.getElementById('pagination-controls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    
    if (!paginationData || !paginationData.totalPages || paginationData.totalPages <= 1) return;
    
    const page = paginationData.page;
    const totalPages = paginationData.totalPages;
    
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = page <= 1;
    prevButton.onclick = () => performSearch(page - 1);
    
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${page} de ${totalPages}`;
    pageInfo.className = 'pagination-info';
    
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = page >= totalPages;
    nextButton.onclick = () => performSearch(page + T1);
    
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}