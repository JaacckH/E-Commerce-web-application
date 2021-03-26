function readURL(input) {
    var reader = new FileReader();
    reader.onload = function (e) {
        document.getElementById("Img").setAttribute("src", e.target.result);
    };
    reader.readAsDataURL(input.files[0]);

}


window.ToggleTabs = ToggleTabs;
function ToggleTabs(n, o, i) {
    const tab_1 = document.querySelector("#panel-tab-" + o + "-" + i);
    tab_1.classList.remove("active");
    const tab_2 = document.querySelector("#panel-tab-" + n + "-" + i);
    tab_2.classList.add("active");

    const panel_1 = document.querySelector("#tab-pane-" + o + "-" + i);
    panel_1.classList.remove("show", "active");
    const panel_2 = document.querySelector("#tab-pane-" + n + "-" + i);
    panel_2.classList.add("show", "active");

}


function ToggleMenu() {
    const element = document.querySelector("#admin-menu");
    element.classList.toggle("d-none");

    const element_two = document.querySelector("#admin-page");
    element_two.classList.toggle("d-none");

}

$(document).ready(function () {
    $("#search-rows").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        if (value.length == 0) {
            $('#refine-search').addClass('d-none');
        } else {
            $('#refine-search').removeClass('d-none');
        }
        var value2 = $("#refine-search").val().toLowerCase();
        $("#data-list li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1 && $(this).text().toLowerCase().indexOf(value2) > -1)
        });
    });
});

$(document).ready(function () {
    $("#refine-search").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        var value2 = $("#search-rows").val().toLowerCase();
        $("#data-list li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1 && $(this).text().toLowerCase().indexOf(value2) > -1)
        });
    });
});

window.OpenDetails = OpenDetails;
function OpenDetails(rowID) {
    $("#search-rows").val(rowID)
    $("#refine-search").val('')
    $("#search-rows").keyup();
    $('#summary-panel-' + rowID).addClass('d-none');
    $('#expanded-panel-' + rowID).removeClass('d-none');
    $('#search-area').addClass('d-none');
    $('#data-header').addClass('d-none');

}

function closeDetails(rowID) {
    $('#summary-panel-' + rowID).removeClass('d-none');
    $('#search-area').removeClass('d-none');
    $('#expanded-panel-' + rowID).addClass('d-none');
    $('#data-header').removeClass('d-none');
    $("#search-rows").val('');
    $("#search-rows").keyup();

}