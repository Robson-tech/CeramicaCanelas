 function setActive(element) {
        document.querySelectorAll('.menu-item').forEach(item => {
            item.classList.remove('active');
        });
        element.classList.add('active');
    }

    function toggleSubmenu(element) {
        const submenu = element.nextElementSibling;
        const icon = element.querySelector('.expand-icon');
        submenu.classList.toggle('expanded');
        icon.classList.toggle('rotated');
    }

    document.addEventListener('DOMContentLoaded', function () {
        const submenu = document.getElementById('almoxarifado-submenu');
        const icon = document.querySelector('.expand-icon');

        if (submenu && icon) {
            submenu.classList.add('expanded');
            icon.classList.add('rotated');
        }

        const firstItem = document.querySelector('.menu-item');
        if (firstItem) {
            firstItem.classList.add('active');
        }
    });