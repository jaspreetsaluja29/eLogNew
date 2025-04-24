// Get the form and its elements
const form = document.getElementById('bunkeringDataFilterForm');
const pageSize = document.getElementById('pageSize');
const currentPage = document.getElementById('currentPage');
const totalPages = document.getElementById('totalPages');
const prevPage = document.getElementById('prevPage');
const nextPage = document.getElementById('nextPage');

// Function to change page
function changePage(direction) {
    // Parse the current page number
    let page = parseInt(currentPage.innerText);
    let maxPage = parseInt(totalPages.innerText);

    // Calculate new page
    page += direction;

    // Validate page bounds
    if (page < 1) page = 1;
    if (page > maxPage) page = maxPage;

    // Create a form data object with existing parameters
    const formData = new FormData(form);

    // Update the page number and page size
    formData.set('pageNumber', page);
    formData.set('pageSize', pageSize.value);

    // Convert form data to query string
    const params = new URLSearchParams(formData);

    // Redirect to the URL with updated parameters
    window.location.href = `${form.action}?${params.toString()}`;
}

// Function to filter table based on search input
function filterTable() {
    const input = document.getElementById('searchInput');
    const filter = input.value.toUpperCase();
    const table = document.getElementById('bunkeringDataReportTable');
    const rows = table.getElementsByTagName('tr');

    // Loop through all table rows, and hide those who don't match the search query
    for (let i = 1; i < rows.length; i++) {
        let found = false;
        const cells = rows[i].getElementsByTagName('td');

        for (let j = 0; j < cells.length; j++) {
            const cell = cells[j];
            if (cell) {
                const txtValue = cell.textContent || cell.innerText;
                if (txtValue.toUpperCase().indexOf(filter) > -1) {
                    found = true;
                    break;
                }
            }
        }

        rows[i].style.display = found ? '' : 'none';
    }
}

// Function to sort table
function sortTable(n) {
    const table = document.getElementById('bunkeringDataReportTable');
    let switching = true;
    let direction = 'asc';
    let shouldSwitch, rows, i, x, y, shouldSwap;

    // Set the sorting direction to ascending
    let switchCount = 0;

    // Continue until no switching is needed
    while (switching) {
        switching = false;
        rows = table.rows;

        // Loop through all table rows except the header
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;

            // Get the two elements to compare
            x = rows[i].getElementsByTagName('td')[n];
            y = rows[i + 1].getElementsByTagName('td')[n];

            // Check if the two rows should switch places
            let xContent = x.innerHTML.toLowerCase();
            let yContent = y.innerHTML.toLowerCase();

            // Convert to numbers for numerical columns
            if (!isNaN(xContent) && !isNaN(yContent)) {
                xContent = parseFloat(xContent);
                yContent = parseFloat(yContent);
            }

            if (direction === 'asc') {
                if (xContent > yContent) {
                    shouldSwitch = true;
                    break;
                }
            } else if (direction === 'desc') {
                if (xContent < yContent) {
                    shouldSwitch = true;
                    break;
                }
            }
        }

        if (shouldSwitch) {
            // If a switch is needed, make the switch and mark that a switch has been done
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;

            // Each time a switch is done, increase the count by 1
            switchCount++;
        } else {
            // If no switching has been done AND the direction is "asc", set the direction to "desc" and run the loop again
            if (switchCount === 0 && direction === 'asc') {
                direction = 'desc';
                switching = true;
            }
        }
    }
}

// Initialize event listeners when the document is ready
document.addEventListener('DOMContentLoaded', function () {
    // Set event listener for page size change
    if (pageSize) {
        pageSize.addEventListener('change', function () {
            changePage(0); // Keep the same page but update page size
        });
    }
});