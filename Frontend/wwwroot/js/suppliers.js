console.log('Script js/supplier.js DEFINIDO.');



// =======================================================
// INICIALIZAÇÃO
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de supplier.js foi chamada.');
    
    initializeSupplierForm(document.querySelector('.supplier-form'));
    initializeSupplierTableFilters();
    fetchAndRenderSuppliers(1);
}

// =======================================================
// LÓGICA DO FORMULÁRIO DE CADASTRO
// =======================================================
function initializeSupplierForm(form) {
    if (!form) return;
    form.onsubmit = (event) => {
        event.preventDefault();
        processAndSendSupplierData(form);
    };
}

async function processAndSendSupplierData(form) {
    const formData = new FormData(form);
    if (!formData.get('Name')?.trim() || !formData.get('Cnpj')?.trim()) {
        showErrorModal({ title: "Validação Falhou", detail: "Nome/Razão Social e CNPJ são obrigatórios." });
        return;
    }

    const submitButton = form.querySelector('.submit-btn');
    const originalButtonHTML = submitButton.innerHTML;
    submitButton.disabled = true;
    submitButton.innerHTML = `<span class="loading-spinner"></span> Salvando...`;

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/supplier`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData,
        });
        if (response.ok) {
            alert('Fornecedor salvo com sucesso!');
            form.reset();
            fetchAndRenderSuppliers(1);
        } else {
            const errorData = await response.json();
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: "Não foi possível comunicar com o servidor." });
    } finally {
        submitButton.disabled = false;
        submitButton.innerHTML = originalButtonHTML;
    }
}
// =======================================================
// LÓGICA DA TABELA (FILTROS, PAGINAÇÃO, CRUD)
// =======================================================
function initializeSupplierTableFilters() {
    const filterBtn = document.getElementById('supplierFilterBtn');
    const clearBtn = document.getElementById('supplierClearFilterBtn');
    
    filterBtn?.addEventListener('click', () => fetchAndRenderSuppliers(1));
    clearBtn?.addEventListener('click', () => {
        document.getElementById('supplierSearchInput').value = '';
        fetchAndRenderSuppliers(1);
    });
}

async function fetchAndRenderSuppliers(page = 1) {
    currentSupplierPage = page;
    const tableBody = document.querySelector('#supplier-list-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Buscando...</td></tr>';
    
    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Não autenticado.");
        
        const search = document.getElementById('supplierSearchInput')?.value;
        const params = new URLSearchParams({ Page: currentSupplierPage, PageSize: 10, OrderBy: 'Name' });
        if (search) params.append('Search', search);

        const url = `${API_BASE_URL}/supplier/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar fornecedores (Status: ${response.status})`);
        
        const paginatedData = await response.json();
        renderSupplierTable(paginatedData.items);
        renderSupplierPagination(paginatedData);
    } catch (error) {
        showErrorModal({ title: "Erro ao Listar Fornecedores", detail: error.message });
        tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: red;">${error.message}</td></tr>`;
    }
}

function renderSupplierTable(suppliers) {
    const tableBody = document.querySelector('#supplier-list-body');
    tableBody.innerHTML = '';
    if (!suppliers || suppliers.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Nenhum fornecedor encontrado.</td></tr>';
        return;
    }
    suppliers.forEach(supplier => {
        const supplierJsonString = JSON.stringify(supplier).replace(/'/g, "&apos;");
        const rowHTML = `
            <tr id="row-supplier-${supplier.id}">
                <td data-field="name">${supplier.name}</td>
                <td data-field="cnpj">${supplier.cnpj || 'N/A'}</td>
                <td data-field="email">${supplier.email || 'N/A'}</td>
                <td data-field="phone">${supplier.phone || 'N/A'}</td>
                <td class="actions-cell">
                    <button class="btn-action btn-edit" onclick='editSupplier(${supplierJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteSupplier('${supplier.id}')">Excluir</button>
                </td>
            </tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

function renderSupplierPagination(paginationData) {
    const controlsContainer = document.getElementById('supplier-pagination-controls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    if (paginationData.totalPages <= 1) return;
    
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = !paginationData.hasPreviousPage;
    prevButton.addEventListener('click', () => fetchAndRenderSuppliers(currentSupplierPage - 1));
    
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${paginationData.page} de ${paginationData.totalPages}`;
    pageInfo.className = 'pagination-info';
    
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = !paginationData.hasNextPage;
    nextButton.addEventListener('click', () => fetchAndRenderSuppliers(currentSupplierPage + 1));
    
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

window.deleteSupplier = async (supplierId) => {
    if (!confirm('Tem certeza que deseja excluir este fornecedor?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/supplier/${supplierId}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (response.ok) {
            alert('Fornecedor excluído com sucesso!');
            fetchAndRenderSuppliers(currentSupplierPage);
        } else {
            const errorData = await response.json();
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
};

window.editSupplier = (supplier) => {
    const row = document.getElementById(`row-supplier-${supplier.id}`);
    if (!row) return;
    originalRowHTML_Supplier[supplier.id] = row.innerHTML;

    row.querySelector('[data-field="name"]').innerHTML = `<input type="text" name="Name" class="edit-input" value="${supplier.name}">`;
    row.querySelector('[data-field="cnpj"]').innerHTML = `<input type="text" name="Cnpj" class="edit-input" value="${supplier.cnpj || ''}">`;
    row.querySelector('[data-field="email"]').innerHTML = `<input type="email" name="Email" class="edit-input" value="${supplier.email || ''}">`;
    row.querySelector('[data-field="phone"]').innerHTML = `<input type="text" name="Phone" class="edit-input" value="${supplier.phone || ''}">`;
    
    row.querySelector('.actions-cell').innerHTML = `
        <button class="btn-action btn-save" onclick="saveSupplierChanges('${supplier.id}')">Salvar</button>
        <button class="btn-action btn-cancel" onclick="cancelEditSupplier('${supplier.id}')">Cancelar</button>`;
};

window.saveSupplierChanges = async (supplierId) => {
    const row = document.getElementById(`row-supplier-${supplierId}`);
    if (!row) return;

    const saveButton = row.querySelector('.btn-save');
    saveButton.disabled = true;
    saveButton.innerHTML = `<span class="loading-spinner"></span>`;

    const formData = new FormData();
    formData.append('Id', supplierId);
    const inputs = row.querySelectorAll('input');
    inputs.forEach(input => formData.append(input.name, input.value));

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/supplier`, {
            method: 'PUT',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData
        });
        if (response.ok) {
            // A recarga da tabela já é um feedback visual, o alert é opcional.
            // alert('Fornecedor atualizado com sucesso!');
            fetchAndRenderSuppliers(currentSupplierPage);
        } else {
            const errorData = await response.json();
            showErrorModal(errorData);
            // Restaura a linha em caso de erro para o usuário poder corrigir.
            cancelEditSupplier(supplierId);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
        cancelEditSupplier(supplierId);
    }
};

window.cancelEditSupplier = (supplierId) => {
    const row = document.getElementById(`row-supplier-${supplierId}`);
    if (row && originalRowHTML_Supplier[supplierId]) {
        row.innerHTML = originalRowHTML_Supplier[supplierId];
        delete originalRowHTML_Supplier[supplierId];
    }
};