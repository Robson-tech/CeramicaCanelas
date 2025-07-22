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
            // ALTERAÇÃO AQUI: colspan mudou de 6 para 5
            tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Buscando histórico...</td></tr>';

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

                historyItemsCache = responseData;
                renderHistoryTable(responseData);
                renderHistoryPagination(null);

            } catch (error) {
                console.error("Erro em fetchAndRenderHistory:", error);
                // ALTERAÇÃO AQUI: colspan mudou de 6 para 5
                tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: red;"><b>Erro ao buscar dados:</b> ${error.message}</td></tr>`;
                document.getElementById('historyPaginationControls').innerHTML = '';
            }
        }

        function renderHistoryTable(items) {
            const tableBody = document.getElementById('historyTbody');
            tableBody.innerHTML = '';
            if (!items || items.length === 0) {
                // ALTERAÇÃO AQUI: colspan mudou de 6 para 5
                tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Nenhum registro encontrado.</td></tr>';
                return;
            }
            items.forEach(item => {
                const exitDate = new Date(item.dataRetirada).toLocaleDateString('pt-BR');
                const actionsCellHTML = `<button class="btn-action btn-return" data-id="${item.id}">Devolver</button>`;
                const row = document.createElement('tr');
                row.id = `row-history-${item.id}`;
                // ALTERAÇÃO AQUI: Removida a célula (td) do operador
                row.innerHTML = `
                    <td data-field="productName">${item.productName || 'N/A'}</td>
                    <td data-field="employeeName">${item.employeeName || 'N/A'}</td>
                    <td data-field="quantity">${item.quantityPendente}</td>
                    <td data-field="exitDate">${exitDate}</td>
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

        async function returnHistoryItem(exitId) {
            const item = historyItemsCache.find(i => String(i.id) === String(exitId));
            if (!item) {
                alert("Erro: Item não encontrado.");
                return;
            }
            if (!confirm(`Deseja confirmar a devolução de ${item.quantityPendente} unidade(s) do produto "${item.productName}"?`)) {
                return;
            }
            try {
                const accessToken = localStorage.getItem('accessToken');
                const response = await fetch(`${API_BASE_URL}/products-exit/${exitId}/return`, {
                    method: 'POST',
                    headers: { 'Authorization': `Bearer ${accessToken}` }
                });
                if (response.ok) {
                    alert('Devolução registrada com sucesso!');
                    fetchAndRenderHistory(currentHistoryPage);
                } else {
                    const errorData = await response.json().catch(() => ({ title: "Erro ao Devolver" }));
                    alert(`Erro ao devolver: ${errorData.title || 'Erro desconhecido'}`);
                }
            } catch (error) {
                alert(`Erro de Conexão: ${error.message}`);
            }
        }

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
                selectElement.innerHTML = `<option value="">Erro ao carregar</option>`;
            }
        }

        document.addEventListener('DOMContentLoaded', initDynamicForm);