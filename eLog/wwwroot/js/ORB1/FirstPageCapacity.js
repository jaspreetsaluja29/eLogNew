$(document).ready(function () {
    let retentionIndex = 1;
    let sludgeIndex = 1;
    let meanIndex = 1;

    function addNewRow(tableId, type) {
        let index = type === 'retentions' ? retentionIndex++ :
            (type === 'sludges' ? sludgeIndex++ : meanIndex++);
        let newRow = `
            <tr>
                <td><input type="text" name="${type}[${index}].TankIdentification" class="form-control" /></td>
                <td><input type="text" name="${type}[${index}].TankLocation_Frames_From_To" class="form-control" /></td>
                <td><input type="text" name="${type}[${index}].TankLocation_LateralPosition" class="form-control" /></td>
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

    $('#addmeanRowButton').on('click', function () {
        addNewRow('#meanTable', 'means');
    });

    $('.table').on('click', '.remove-row', function () {
        $(this).closest('tr').remove();
    });

    $('#capacityForm').on('submit', function (e) {
        e.preventDefault();

        let retentionData = [];
        $('#retentionTable tbody tr').each(function () {
            let volume = $(this).find('input[name^="retentions"]').eq(3).val();

            // Only add the row if at least one field is filled
            if ($(this).find('input').filter(function () { return this.value.trim() !== ""; }).length > 0) {
                retentionData.push({
                    TankIdentification: $(this).find('input[name^="retentions"]').eq(0).val(),
                    TankLocation_Frames_From_To: $(this).find('input[name^="retentions"]').eq(1).val(),
                    TankLocation_LateralPosition: $(this).find('input[name^="retentions"]').eq(2).val(),
                    Volume_m3: volume !== "" ? parseFloat(volume) : null // Use null instead of 0 for empty values
                });
            }
        });

        let sludgeData = [];
        $('#sludgeTable tbody tr').each(function () {
            let volume = $(this).find('input[name^="sludges"]').eq(3).val();

            // Only add the row if at least one field is filled
            if ($(this).find('input').filter(function () { return this.value.trim() !== ""; }).length > 0) {
                sludgeData.push({
                    TankIdentification: $(this).find('input[name^="sludges"]').eq(0).val(),
                    TankLocation_Frames_From_To: $(this).find('input[name^="sludges"]').eq(1).val(),
                    TankLocation_LateralPosition: $(this).find('input[name^="sludges"]').eq(2).val(),
                    Volume_m3: volume !== "" ? parseFloat(volume) : null // Keep it null instead of 0
                });
            }
        });

        let meanData = [];
        $('#meanTable tbody tr').each(function () {
            let volume = $(this).find('input[name^="means"]').eq(3).val();

            // Only add the row if at least one field is filled
            if ($(this).find('input').filter(function () { return this.value.trim() !== ""; }).length > 0) {
                meanData.push({
                    TankIdentification: $(this).find('input[name^="means"]').eq(0).val(),
                    TankLocation_Frames_From_To: $(this).find('input[name^="means"]').eq(1).val(),
                    TankLocation_LateralPosition: $(this).find('input[name^="means"]').eq(2).val(),
                    Volume_m3: volume !== "" ? parseFloat(volume) : null // Use null instead of 0 for empty values
                });
            }
        });

        var basePath = document.querySelector('base')?.getAttribute('href') || '/';

        $.ajax({
            url: basePath + "/FirstPageCapacity/SaveData",
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded',
            data: { retentions: retentionData, sludges: sludgeData, means: meanData},
            success: function (response) {
                alert('Data saved successfully!');
                window.location.href = `${basePath}/FirstPageCapacity/FirstPageCapacity`;
            },
            error: function () {
                alert('Failed to save data.');
            }
        });
    });
});