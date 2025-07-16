console.log('Script js/product.js DEFINIDO.');

const API_BASE_URL = 'http://localhost:5087/api';
const originalRowHTML = {};
let currentTablePage = 1;

// =======================================================
// LÓGICA DO FORMULÁRIO DE CADASTRO
// =======================================================
async function loadProductCategories(selectElement, defaultOptionText = 'Selecione uma categoria') {
    if (!selectElement) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/categories`, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error('Falha ao carregar categorias.');
        const categories = await response.json();
        selectElement.innerHTML = `<option value="">${defaultOptionText}</option>`;
        categories.forEach(category => {
            const option = new Option(category.name, category.id);
            selectElement.appendChild(option);
        });
    } catch (error) {
        console.error('Erro ao carregar categorias:', error);
        selectElement.innerHTML = '<option value="">Erro ao carregar</option>';
    }
}

function initializeProductForm(form) {
    if (!form) return;
    form.addEventListener('submit', (event) => {
        event.preventDefault();
        processAndSendProductData(form);
    });
}

async function processAndSendProductData(form) {
    const formData = new FormData(form);
    if (!formData.get('Code')?.trim() || !formData.get('Name')?.trim() || !formData.get('CategoryId')) {
        alert('Por favor, preencha os campos obrigatórios: Código, Nome e Categoria.');
        return;
    }
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData,
        });
        if (response.ok) {
            alert('Produto cadastrado com sucesso!');
            form.reset();
            fetchAndRenderProducts(1);
        } else {
            const errorData = await response.json();
            alert(`Erro: ${errorData.title || errorData.message || 'Erro ao salvar o produto.'}`);
        }
    } catch (error) {
        console.error('❌ Erro na requisição de cadastro:', error);
        alert('Falha na comunicação com o servidor.');
    }
}


// =======================================================
// LÓGICA DA TABELA (COM FILTROS E PAGINAÇÃO CORRIGIDOS)
// =======================================================

async function fetchAndRenderProducts(page = 1) {
    currentTablePage = page;
    const tableBody = document.querySelector('#product-list-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="8" style="text-align: center;">Buscando...</td></tr>';
    
    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Não autenticado.");

        const search = document.getElementById('searchInput')?.value;
        const categoryId = document.getElementById('categoryFilter')?.value;
        
        const params = new URLSearchParams({
            Page: currentTablePage,
            PageSize: 4
        });

        if (search) {
            params.append('Search', search);
        }
        if (categoryId) {
            params.append('CategoryId', categoryId);
        }
        
        const url = `${API_BASE_URL}/products/paged?${params.toString()}`;
        console.log("📡 Enviando requisição GET para:", url);

        const response = await fetch(url, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });
        
        if (!response.ok) {
            throw new Error(`Falha ao buscar produtos (Status: ${response.status})`);
        }

        const paginatedData = await response.json();
        renderProductTable(paginatedData.items);
        renderPagination(paginatedData);

    } catch (error) {
        console.error("❌ Erro ao buscar produtos:", error);
        tableBody.innerHTML = `<tr><td colspan="8" style="text-align: center; color: red;">${error.message}</td></tr>`;
        const paginationControls = document.getElementById('pagination-controls');
        if(paginationControls) paginationControls.innerHTML = '';
    }
}

function renderProductTable(products) {
    const tableBody = document.querySelector('#product-list-body');
    tableBody.innerHTML = '';
    if (!products || products.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="8" style="text-align: center;">Nenhum produto encontrado.</td></tr>';
        return;
    }
    products.forEach(product => {
        const imageUrl = product.imageUrl || 'https://via.placeholder.com/60';
        const formattedValue = (product.value || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        const productJsonString = JSON.stringify(product).replace(/'/g, "&apos;");
        const rowHTML = `
            <tr id="row-${product.id}">
                <td><img src="${imageUrl}" alt="${product.name}" class="product-table-img"></td>
                <td data-field="code">${product.code || 'N/A'}</td>
                <td data-field="name">${product.name}</td>
                <td data-field="category" data-category-id="${product.categoryId}">${product.categoryName || 'N/A'}</td>
                <td data-field="stock">${product.stockCurrent || 0}</td>
                <td data-field="minStock">${product.stockMinium || 0}</td>
                <td data-field="value">${formattedValue}</td>
                <td class="actions-cell" data-field="actions">
                    <button class="btn-action btn-edit" onclick='editProduct(${productJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteProduct('${product.id}')">Excluir</button>
                </td>
            </tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

function renderPagination(paginationData) {
    const controlsContainer = document.getElementById('pagination-controls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    if (!paginationData || paginationData.totalPages <= 1) return;
    const hasPreviousPage = paginationData.page > 1;
    const hasNextPage = paginationData.page < paginationData.totalPages;
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = !hasPreviousPage;
    prevButton.addEventListener('click', () => fetchAndRenderProducts(currentTablePage - 1));
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${paginationData.page} de ${paginationData.totalPages}`;
    pageInfo.className = 'pagination-info';
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = !hasNextPage;
    nextButton.addEventListener('click', () => fetchAndRenderProducts(currentTablePage + 1));
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

window.deleteProduct = async (productId) => {
    if (!confirm('Tem certeza que deseja excluir este produto?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products/${productId}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (response.ok) {
            alert('Produto excluído com sucesso!');
            fetchAndRenderProducts(currentTablePage);
        } else {
            throw new Error('Falha ao excluir o produto.');
        }
    } catch (error) {
        alert(error.message);
    }
};

window.editProduct = async (product) => {
    const row = document.getElementById(`row-${product.id}`);
    if (!row) return;
    originalRowHTML[product.id] = row.innerHTML;
    row.querySelector('[data-field="code"]').innerHTML = `<input type="text" name="Code" class="edit-input" value="${product.code || ''}">`;
    row.querySelector('[data-field="name"]').innerHTML = `<input type="text" name="Name" class="edit-input" value="${product.name}">`;
    row.querySelector('[data-field="minStock"]').innerHTML = `<input type="number" name="StockMinium" class="edit-input" value="${product.stockMinium || 0}">`;
    row.querySelector('[data-field="value"]').innerHTML = `<input type="number" step="0.01" name="Value" class="edit-input" value="${product.value || 0}">`;
    const categoryCell = row.querySelector('[data-field="category"]');
    const categorySelect = document.createElement('select');
    categorySelect.className = 'edit-input';
    categorySelect.name = 'CategoryId';
    categoryCell.innerHTML = '';
    categoryCell.appendChild(categorySelect);
    await loadProductCategories(categorySelect);
    categorySelect.value = product.categoryId;
    row.querySelector('[data-field="actions"]').innerHTML = `
        <button class="btn-action btn-save" onclick="saveProductChanges('${product.id}')">Salvar</button>
        <button class="btn-action btn-cancel" onclick="cancelEdit('${product.id}')">Cancelar</button>
    `;
};

window.saveProductChanges = async (productId) => {
    const row = document.getElementById(`row-${productId}`);
    if (!row) return;
    const formData = new FormData();
    formData.append('Id', productId);
    const inputs = row.querySelectorAll('input, select');
    inputs.forEach(input => {
        if (input.name === 'Value') {
            formData.append(input.name, input.value.replace(',', '.'));
        } else {
            formData.append(input.name, input.value);
        }
    });
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products`, { method: 'PUT', headers: { 'Authorization': `Bearer ${accessToken}` }, body: formData });
        if (response.ok) {
            alert('Produto atualizado com sucesso!');
            fetchAndRenderProducts(currentTablePage);
        } else {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Falha ao atualizar o produto.');
        }
    } catch (error) {
        alert(error.message);
        cancelEdit(productId);
    }
};

window.cancelEdit = (productId) => {
    const row = document.getElementById(`row-${productId}`);
    if (row && originalRowHTML[productId]) {
        row.innerHTML = originalRowHTML[productId];
        delete originalRowHTML[productId];
    }
};

// =======================================================
// INICIALIZAÇÃO DA PÁGINA
// =======================================================

function initDynamicForm() {
    console.log('▶️ initDynamicForm() de product.js foi chamada.');
    
    const formElement = document.querySelector('.product-form');
    initializeProductForm(formElement);
    loadProductCategories(document.querySelector('select[name="CategoryId"]'));

    const filterBtn = document.getElementById('filterBtn');
    const clearFilterBtn = document.getElementById('clearFilterBtn');
    
    if(filterBtn) filterBtn.addEventListener('click', () => fetchAndRenderProducts(1));
    if(clearFilterBtn) clearFilterBtn.addEventListener('click', () => {
        document.getElementById('searchInput').value = '';
        document.getElementById('categoryFilter').value = '';
        fetchAndRenderProducts(1);
    });
    
    const categoryFilterSelect = document.querySelector('#categoryFilter');
    if (categoryFilterSelect) {
        loadProductCategories(categoryFilterSelect, 'Todas as Categorias')
            .then(() => {
                fetchAndRenderProducts(1);
            });
    } else {
        fetchAndRenderProducts(1); 
    }
}