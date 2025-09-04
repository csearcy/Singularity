$(document).ready(function () {    
    const $dropdown = $('#raid-dropdown');
    if ($dropdown.length === 0) return;

    const initialRaid = $dropdown.find('option:selected').text(); 

    if (initialRaid) {
        $('#main-content').hide();
        showLoadingOverlay();

        $.ajax({
            url: '/api/data/load',
            method: 'GET',
            data: { raidName: initialRaid },
            success: function () {
                checkDataReady(initialRaid);
            },
            error: function () {
                hideLoadingOverlay();
            }
        });
    }
});

function showLoadingOverlay() {
    $('#main-content').hide();

    if ($('.data-loading-overlay').length === 0) {
        $.get('/Race?handler=DataLoading', function (html) {
            $("#loadingcontent").append(html);
        });
    } else {
        $('.data-loading-overlay').show();
    }

    $('body').addClass('loading-cursor');
}

function hideLoadingOverlay() {
    $('.data-loading-overlay').remove();
    $('body').removeClass('loading-cursor');
    $('#main-content').show();
}

function loadRaidAndWait(raidName, slug) {
    showLoadingOverlay();

    $.ajax({
        url: `/api/data/load`,
        method: 'GET',
        data: { raidName: raidName },
        success: function () {
            pollDataReady(raidName, slug);
        },
        error: function () {
            hideLoadingOverlay();
            alert("Failed to load raid data.");
        }
    });
}

function pollDataReady(raidName, slug) {
    $.get(`/Race?handler=RaceTablesPartial&raidName=${encodeURIComponent(raidName)}`, function(html) {
        $('#main-content').html(html);        
        if (raidName) {
            $('#raid-dropdown').val(slug);
        }

        hideLoadingOverlay();
    }).fail(function() {
        hideLoadingOverlay();
    });
}

$(document).on("change", '#raid-dropdown', function () {
    const raidName = $(this).find('option:selected').text();
    const raidSlug = $(this).find('option:selected').val();
    loadRaidAndWait(raidName, raidSlug);
});