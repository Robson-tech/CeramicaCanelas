console.log('Script js/product-entry.js DEFINIDO.');

// Este script utiliza as variáveis e funções globais definidas em main.js
// como API_BASE_URL e showErrorModal.


let currentModalPage = 1;

// =======================================================
// INICIALIZAÇÃO DA PÁGINA
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de product-entry.js foi chamada.');
    const formElement = document.querySelector('#productEntryForm');
    initializeFormListeners(formElement);
    fetchAndRenderEntries();
}

// =======================================================
// LÓGICA DO FORMULÁRIO DE ENTRADA E MODAL
// =======================================================

function initializeFormListeners(form) {
    if (!form) return;
    const modal = document.getElementById('productSearchModal');
    const openModalBtn = document.getElementById('openProductModalBtn');
    const closeModalBtn = modal.querySelector('.modal-close-btn');

    openModalBtn.addEventListener('click', () => {
        modal.style.display = 'block';
        currentModalPage = 1;
        fetchAndRenderProductsInModal();
    });
    closeModalBtn.addEventListener('click', () => {
        modal.style.display = 'none';
    });
    window.addEventListener('click', (event) => {
        if (event.target === modal) {
            modal.style.display = 'none';
        }
    });

    initializeMainFormSubmit(form);
    initializeProductSelectionListener(modal);
}

async function fetchAndRenderProductsInModal() {
    const resultsContainer = document.getElementById('modalResultsContainer');
    resultsContainer.innerHTML = '<p>Buscando...</p>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Token não encontrado.");
        const params = new URLSearchParams({ Page: currentModalPage, PageSize: 10 });
        const url = `${API_BASE_URL}/products/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha na requisição: ${response.status}`);
        const paginatedData = await response.json();
        renderModalResults(paginatedData.items, resultsContainer);
        renderModalPagination(paginatedData);
    } catch (error) {
        resultsContainer.innerHTML = `<p style="color:red;">${error.message}</p>`;
    }
}

function renderModalResults(products, container) {
    if (!products || products.length === 0) {
        container.innerHTML = '<p>Nenhum produto encontrado.</p>';
        return;
    }
    const table = document.createElement('table');
    table.className = 'results-table';
    table.innerHTML = `
        <thead><tr><th>Nome</th><th>Código</th><th>Estoque</th><th>Ação</th></tr></thead>
        <tbody>
            ${products.map(product => `
                <tr>
                    <td>${product.name}</td>
                    <td>${product.code || 'N/A'}</td>
                    <td>${product.stockCurrent || 0}</td>
                    <td><button type="button" class="select-product-btn" data-id="${product.id}" data-name="${product.name}">Selecionar</button></td>
                </tr>
            `).join('')}
        </tbody>`;
    container.innerHTML = '';
    container.appendChild(table);
}

function renderModalPagination(paginationData) {
    const controlsContainer = document.getElementById('modalPaginationControls');
    controlsContainer.innerHTML = '';
    if (paginationData.totalPages <= 1) return;
    const hasPreviousPage = paginationData.page > 1;
    const hasNextPage = paginationData.page < paginationData.totalPages;
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = !hasPreviousPage;
    prevButton.addEventListener('click', () => { currentModalPage--; fetchAndRenderProductsInModal(); });
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${paginationData.page} de ${paginationData.totalPages}`;
    pageInfo.className = 'pagination-info';
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = !hasNextPage;
    nextButton.addEventListener('click', () => { currentModalPage++; fetchAndRenderProductsInModal(); });
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
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
        const formData = new FormData(form);
        if (!formData.get('ProductId')) {
            showErrorModal({ title: "Validação Falhou", detail: "Por favor, busque e selecione um produto." });
            return;
        }
        try {
            const accessToken = localStorage.getItem('accessToken');
            const response = await fetch(`${API_BASE_URL}/products-entry`, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${accessToken}` },
                body: formData
            });
            if (response.ok) {
                alert('Entrada registrada com sucesso!');
                form.reset();
                document.getElementById('selectedProductName').textContent = 'Nenhum produto selecionado';
                fetchAndRenderEntries();
            } else {
                const errorData = await response.json();
                showErrorModal(errorData);
            }
        } catch (error) {
            showErrorModal({ title: "Erro de Conexão", detail: "Falha na comunicação com a API ao registrar a entrada." });
        }
    });
}

// =======================================================
// LÓGICA DA TABELA DE ENTRADAS (CRUD)
// =======================================================
async function fetchAndRenderEntries() {
    const tableBody = document.querySelector('#entry-list-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Buscando...</td></tr>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products-entry`, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error('Falha ao buscar as entradas de produtos.');
        const entries = await response.json();
        renderEntryTable(entries, tableBody);
    } catch (error) {
        showErrorModal({ title: "Erro ao Listar Entradas", detail: error.message });
        tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: red;">Falha ao carregar.</td></tr>`;
    }
}

function renderEntryTable(entries, tableBody) {
    tableBody.innerHTML = '';
    if (!entries || entries.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Nenhuma entrada registrada.</td></tr>';
        return;
    }
    entries.forEach(entry => {
        const entryJsonString = JSON.stringify(entry).replace(/'/g, "&apos;");
        const formattedPrice = (entry.unitPrice || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        const formattedDate = new Date(entry.entryDate).toLocaleDateString('pt-BR');
        const rowHTML = `<tr id="row-entry-${entry.id}"><td data-field="productName">${entry.productName}</td><td data-field="quantity">${entry.quantity}</td><td data-field="unitPrice">${formattedPrice}</td><td data-field="entryDate">${formattedDate}</td><td class="actions-cell" data-field="actions"><button class="btn-action btn-edit" onclick='editEntry(${entryJsonString})'>Editar</button><button class="btn-action btn-delete" onclick="deleteEntry('${entry.id}')">Excluir</button></td></tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

window.deleteEntry = async (entryId) => {
    if (!confirm('Tem certeza?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products-entry/${entryId}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (response.ok) {
            alert('Entrada excluída!');
            fetchAndRenderEntries();
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Excluir", detail: "Ocorreu um erro inesperado."}));
            showErrorModal(errorData);
        }
    } catch (error) { 
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
};

window.editEntry = (entry) => {
    const row = document.getElementById(`row-entry-${entry.id}`);
    if (!row) return;
    originalEntryRowHTML[entry.id] = row.innerHTML;
    row.querySelector('[data-field="quantity"]').innerHTML = `<input type="number" name="Quantity" class="edit-input" value="${entry.quantity}">`;
    row.querySelector('[data-field="unitPrice"]').innerHTML = `<input type="number" step="0.01" name="UnitPrice" class="edit-input" value="${entry.unitPrice}">`;
    row.querySelector('[data-field="actions"]').innerHTML = `<button class="btn-action btn-save" onclick="saveEntryChanges('${entry.id}')">Salvar</button><button class="btn-action btn-cancel" onclick="cancelEntryEdit('${entry.id}')">Cancelar</button>`;
};

window.saveEntryChanges = async (entryId) => {
    const row = document.getElementById(`row-entry-${entryId}`);
    if (!row) return;
    const payload = {
        id: entryId,
        quantity: parseInt(row.querySelector('[name="Quantity"]').value, 10),
        unitPrice: parseFloat(row.querySelector('[name="UnitPrice"]').value.replace(',', '.'))
    };
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products-entry`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${accessToken}` },
            body: JSON.stringify(payload)
        });
        if (response.ok) {
            alert('Entrada atualizada!');
            fetchAndRenderEntries();
        } else { 
            const errorData = await response.json().catch(() => ({ title: "Erro ao Salvar", detail: "Ocorreu um erro inesperado."}));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
        cancelEntryEdit(entryId);
    }
};

window.cancelEntryEdit = (entryId) => {
    const row = document.getElementById(`row-entry-${entryId}`);
    if (row && originalEntryRowHTML[entryId]) {
        row.innerHTML = originalEntryRowHTML[entryId];
        delete originalEntryRowHTML[entryId];
    }
};