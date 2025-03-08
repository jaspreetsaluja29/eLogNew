$(document).ready(function () {
    // Add new row
    $('#addRowButton').on('click', function () {
        let newRow = `
            <tr>
                <td><input type="text" name="TankIdentification" class="form-control" /></td>
                <td><input type="text" name="TankLocation_Frames_From_To" class="form-control" /></td>
                <td><input type="number" step="0.01" name="Volume_m3" class="form-control" /></td>
                <td><button type="button" class="btn btn-danger remove-row">Remove</button></td>
            </tr>
        `;
        $('#retentionTable tbody').append(newRow);
    });

    // Remove row
    $('#retentionTable').on('click', '.remove-row', function () {
        $(this).closest('tr').remove();
    });

    // AJAX form submission
    $('#retentionForm').on('submit', function (e) {
        e.preventDefault();

        let formData = [];
        $('#retentionTable tbody tr').each(function () {
            formData.push({
                TankIdentification: $(this).find('input[name="TankIdentification"]').val(),
                TankLocation_Frames_From_To: $(this).find('input[name="TankLocation_Frames_From_To"]').val(),
                Volume_m3: $(this).find('input[name="Volume_m3"]').val()
            });
        });

        $.ajax({
            url: '/FirstPageCapacity/SaveData',
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded', // Important for model binding
            data: { retentions: formData }, // Send as key-value pair
            success: function (response) {
                alert('Data saved successfully!');

                window.location.href = `/FirstPageCapacity/FirstPageCapacity`;
            },
            error: function () {
                alert('Failed to save data.');
            }
        });
    });

});
