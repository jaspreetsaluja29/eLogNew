$(document).ready(function () {
    let retentionIndex = 1;
    let sludgeIndex = 1;

    function addNewRow(tableId, type) {
        let index = type === 'retentions' ? retentionIndex++ : sludgeIndex++;
        let newRow = `
            <tr>
                <td><input type="text" name="${type}[${index}].TankIdentification" class="form-control" /></td>
                <td><input type="text" name="${type}[${index}].TankLocation_Frames_From_To" class="form-control" /></td>
                <td><input type="number" step="0.01" name="${type}[${index}].Volume_m3" class="form-control" /></td>
                <td><button type="button" class="btn btn-danger remove-row">Remove</button></td>
            </tr>
        `;
        $(tableId + ' tbody').append(newRow);
    }

    $('#addRowButton').on('click', function () {
        addNewRow('#retentionTable', 'retentions');
    });

    $('#addSludgeRowButton').on('click', function () {
        addNewRow('#sludgeTable', 'sludges');
    });

    $('.table').on('click', '.remove-row', function () {
        $(this).closest('tr').remove();
    });

    $('#capacityForm').on('submit', function (e) {
        e.preventDefault();

        let retentionData = [];
        $('#retentionTable tbody tr').each(function () {
            retentionData.push({
                TankIdentification: $(this).find('input[name^="retentions"]').eq(0).val(),
                TankLocation_Frames_From_To: $(this).find('input[name^="retentions"]').eq(1).val(),
                Volume_m3: $(this).find('input[name^="retentions"]').eq(2).val()
            });
        });

        let sludgeData = [];
        $('#sludgeTable tbody tr').each(function () {
            sludgeData.push({
                TankIdentification: $(this).find('input[name^="sludges"]').eq(0).val(),
                TankLocation_Frames_From_To: $(this).find('input[name^="sludges"]').eq(1).val(),
                Volume_m3: $(this).find('input[name^="sludges"]').eq(2).val()
            });
        });

        $.ajax({
            url: '/FirstPageCapacity/SaveData',
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded',
            data: { retentions: retentionData, sludges: sludgeData },
            success: function (response) {
                alert('Data saved successfully!');
                window.location.href = '/FirstPageCapacity/FirstPageCapacity';
            },
            error: function () {
                alert('Failed to save data.');
            }
        });
    });
});
