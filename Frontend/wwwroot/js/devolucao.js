console.log('Script de devolução INICIADO.');

function initDynamicForm() {
    console.log('▶️ initDynamicForm() foi chamada.');
    initializeHistoryFilters();
    initializeHistoryTableListeners();
    loadProductCategories(document.getElementById('historyCategoryFilter'), 'Todas as Categorias')
        .then(() => fetchAndRenderHistory(1))
        .catch(err => console.error("Erro final ao inicializar:", err));
}

function initializeHistoryFilters() {
    const filterBtn = document.getElementById('historyFilterBtn');
    const clearBtn = document.getElementById('historyClearFilterBtn');
    if (filterBtn) filterBtn.addEventListener('click', () => fetchAndRenderHistory(1));
    if (clearBtn) {
        clearBtn.addEventListener('click', () => {
            document.getElementById('historySearchInput').value = '';
            document.getElementById('historyCategoryFilter').value = '';
            document.getElementById('historyEmployeeNameFilter').value = '';
            document.getElementById('historyStartDateFilter').value = '';
            document.getElementById('historyEndDateFilter').value = '';
            fetchAndRenderHistory(1);
        });
    }
}

function initializeHistoryTableListeners() {
    const tableBody = document.getElementById('historyTbody');
    if (!tableBody) return;
    tableBody.addEventListener('click', (event) => {
        const target = event.target;
        if (target.classList.contains('btn-return')) {
            event.preventDefault();
            const exitId = target.dataset.id;
            returnHistoryItem(exitId);
        }
    });
}

async function fetchAndRenderHistory(page = 1) {
    currentHistoryPage = page;
    const tableBody = document.getElementById('historyTbody');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Buscando histórico...</td></tr>';

    try {
        const accessToken = localStorage.getItem('accessToken');
        const params = new URLSearchParams({ Page: currentHistoryPage, PageSize: "10" });
        const search = document.getElementById('historySearchInput')?.value;
        const categoryId = document.getElementById('historyCategoryFilter')?.value;
        const employeeName = document.getElementById('historyEmployeeNameFilter')?.value;
        const startDate = document.getElementById('historyStartDateFilter')?.value;
        const endDate = document.getElementById('historyEndDateFilter')?.value;

        if (search) params.append('Search', search);
        if (categoryId) params.append('CategoryId', categoryId);
        if (employeeName) params.append('EmployeeName', employeeName);
        if (startDate) params.append('StartDate', startDate);
        if (endDate) params.append('EndDate', endDate);

        const url = `${API_BASE_URL}/dashboard/reports/products/unreturned-products?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });

        if (!response.ok) throw new Error(`Falha ao buscar histórico (Status: ${response.status})`);
        
        const responseData = await response.json();
        console.log('✅ DADOS RECEBIDOS DA API:', responseData);

        const items = responseData.items || responseData; 
        const pagination = responseData.pagination || { page: currentHistoryPage, totalPages: 1 };

        historyItemsCache = items;
        renderHistoryTable(items);
        renderHistoryPagination(pagination);

    } catch (error) {
        console.error("Erro em fetchAndRenderHistory:", error);
        tableBody.innerHTML = `<tr><td colspan="6" style="text-align: center; color: red;"><b>Erro ao buscar dados:</b> ${error.message}</td></tr>`;
        document.getElementById('historyPaginationControls').innerHTML = '';
    }
}

function renderHistoryTable(items) {
    const tableBody = document.getElementById('historyTbody');
    tableBody.innerHTML = '';
    if (!items || items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Nenhum registro encontrado.</td></tr>';
        return;
    }
    items.forEach(item => {
        const exitDate = new Date(item.dataRetirada).toLocaleDateString('pt-BR');
        const actionsCellHTML = `<button class="btn-action btn-return" data-id="${item.id}">Devolver</button>`;
        const row = document.createElement('tr');
        row.id = `row-history-${item.id}`;
        
        row.innerHTML = `
            <td data-field="productName">${item.productName || 'N/A'}</td>
            <td data-field="employeeName">${item.employeeName || 'N/A'}</td>
            <td data-field="quantity">${item.quantityPendente}</td>
            <td data-field="exitDate">${exitDate}</td>
            <td data-field="returnQuantity">
                <input 
                    type="number" 
                    class="return-quantity-input" 
                    value="${item.quantityPendente}" 
                    min="1" 
                    max="${item.quantityPendente}" 
                    data-id="${item.id}"
                    style="width: 80px; text-align: center;"
                >
            </td>
            <td class="actions-cell" data-field="actions">${actionsCellHTML}</td>
        `;
        tableBody.appendChild(row);
    });
}

function renderHistoryPagination(paginationData) {
    const controlsContainer = document.getElementById('historyPaginationControls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    if (!paginationData || !paginationData.totalPages || paginationData.totalPages <= 1) return;
    
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = paginationData.page <= 1;
    prevButton.addEventListener('click', () => fetchAndRenderHistory(paginationData.page - 1));

    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${paginationData.page} de ${paginationData.totalPages}`;

    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = paginationData.page >= paginationData.totalPages;
    nextButton.addEventListener('click', () => fetchAndRenderHistory(paginationData.page + 1));

    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

// =================================================================================
// ALTERAÇÃO NESTA FUNÇÃO PARA USAR O NOVO ENDPOINT PUT
// =================================================================================
async function returnHistoryItem(exitId) {
    const item = historyItemsCache.find(i => String(i.id) === String(exitId));
    if (!item) {
        alert("Erro: Item não encontrado no cache.");
        return;
    }

    const quantityInput = document.querySelector(`.return-quantity-input[data-id="${exitId}"]`);
    const quantityToReturn = parseInt(quantityInput.value, 10);

    if (isNaN(quantityToReturn) || quantityToReturn <= 0 || quantityToReturn > item.quantityPendente) {
        alert(`Por favor, insira uma quantidade válida para devolver (entre 1 e ${item.quantityPendente}).`);
        return;
    }

    if (!confirm(`Deseja confirmar a devolução de ${quantityToReturn} unidade(s) do produto "${item.productName}"?`)) {
        return;
    }

    try {
        const accessToken = localStorage.getItem('accessToken');
        
        // 1. Criar um objeto FormData para enviar como multipart/form-data
        const formData = new FormData();
        formData.append('Id', exitId); // Adiciona o ID da saída do produto
        formData.append('QuantityReturned', quantityToReturn); // Adiciona a quantidade devolvida

        // 2. Montar a requisição com o novo endpoint e método
        const response = await fetch(`${API_BASE_URL}/products-exit/returned-products`, {
            method: 'PUT', // Método alterado para PUT
            headers: { 
                'Authorization': `Bearer ${accessToken}`
                // ATENÇÃO: Não defina o 'Content-Type'. O navegador o fará
                // automaticamente com o 'boundary' correto para FormData.
            },
            body: formData // Corpo da requisição é o objeto FormData
        });

        if (response.ok) {
            alert('Devolução registrada com sucesso!');
            fetchAndRenderHistory(currentHistoryPage);
        } else {
            // Tenta ler a resposta como JSON, se falhar, mostra erro genérico
            const errorData = await response.json().catch(() => ({ title: `Erro ${response.status}` }));
            alert(`Erro ao devolver: ${errorData.title || 'Erro desconhecido'}`);
        }
    } catch (error) {
        console.error("Erro de conexão ao devolver item:", error);
        alert(`Erro de Conexão: ${error.message}`);
    }
}
// =================================================================================
// FIM DA FUNÇÃO ALTERADA
// =================================================================================
