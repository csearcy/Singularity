
$(document).ready(function () {
    function toggleRightMenuLinks() {
        if ($('.show-RightMenu3').length > 0) {
            $('#RightMenu3').show();
        } else {
            $('#RightMenu3').hide();
        }
    }

    toggleRightMenuLinks();

    $('.nav-link').on('click', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');

        $('#main-content').load(url + ' #main-content', function () {
            toggleRightMenuLinks();
        });
    });

    window.checkDataReady = function (raidName) {
        if (typeof requiresData !== 'undefined' && requiresData) {
            $('body').addClass('loading-cursor');

            $.ajax({
                url: '/api/data/status',
                method: 'GET',
                success: function (response) {
                    if (response.isReady) {
                        let query = raidName ? `?raidName=${encodeURIComponent(raidName)}` : '';
                        $.get(`/Race?handler=RaceTablesPartial${query}`, function (html) {
                            $('#main-content').html(html).show();
                            $('.data-loading-overlay').remove();
                            $('body').removeClass('loading-cursor');
                        });
                    } else {
                        setTimeout(checkDataReady(raidName), 3000);
                    }
                },
                error: function () {
                    $('body').removeClass('loading-cursor');
                }
            });
        }
    }

    checkDataReady();
});

// Progress bar update function
const bossProgress = document.getElementById('boss-progress');

function updateBossProgressClass() {
  if (bossProgress.value === 0) {
    bossProgress.classList.add('progress-bar-empty');
    bossProgress.classList.remove('progress-bar');
  } else {
    bossProgress.classList.remove('progress-bar-empty');
    bossProgress.classList.add('progress-bar');
  }
}

