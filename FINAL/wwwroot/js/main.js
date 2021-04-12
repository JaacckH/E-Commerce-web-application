
$('#category-select option').mousedown(function (e) {
    e.preventDefault();
    $(this).prop('selected', !$(this).prop('selected'));
    return false;
});


$(document).ready(function () {
var productTags = ['Teal', 'Blue', 'White', 'Long', 'Summer', 'Autumn', 'Winter', 'Warm', 'V-neck', 'Relaxed-Fit', 'long-Sleeve'
];

var productTags = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.whitespace,
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    local: productTags
});

$('#product-tags').tagsinput({
    typeaheadjs: [{
        hint: true,
        highlight: true,
        minLength: 1
    },
    {
        name: 'productTags',
        source: productTags
    }],
    freeInput: true,
    tagClass: 'badge',
    capitalize: function (item) {
        return item ? item.charAt(0).toUpperCase() + item.slice(1).toLowerCase() : item;
    }
});


$('#product-tags').on("beforeItemAdd", function (e) {
    var name = e.item;
    var allow = true;
    $(this).prev().find(".tag").each(function (i, valor) {
        if ($(this).text().toLowerCase() == name.toString().toLowerCase()) {
            allow = false;
            return false;
        }
    })

    if (!allow) {
        console.log("repeat");
        e.cancel = true;
    }
});

});



var imageUploadCount = 0;
window.AddImageUpload = AddImageUpload;
function AddImageUpload(input) {
    var reader = new FileReader();
    reader.onload = function (e) {


        var $image_upload = '               <div id="uploaded-image-' + imageUploadCount + '" class="img-column contains" onclick="RemoveUpload(' + imageUploadCount + ',1)">';
        $image_upload += '                 <img class="img-object" src="' + e.target.result + '" alt="...">';
        $image_upload += '                 <div class="delete-img-btn"><i class="fas fa-times"></i></div>';
        $image_upload += '                 <input id="new-image' + imageUploadCount + '" type="text" value="' + e.target.result + '" hidden>';
        $image_upload += '              </div>';

        $($image_upload).insertBefore($('.img-upload-btn'));

    };
    reader.readAsDataURL(input.files[0]);
    imageUploadCount++;
}


function RemoveUpload(imageID, n) {
    //existing image
    if (n == 0) {
        $("#existing-image-" + imageID).remove();
        var $remove_image = '<input hidden type="text" id="removed-image-' + imageID + '" value="' + imageID + '"/>';
        //$("#image-upload").append($remove_image);
        $($remove_image).insertAfter($('#image-upload'));
    }
    // new upload
    if (n == 1) {
        $("#uploaded-image-" + imageID).remove();
    }

}

function OpenProductDetails(rowID) {
    if ($("#card-lrg-detail-" + rowID).hasClass("d-none")) {
        $("#search-rows").val(rowID)
        $("#refine-search").val('')
        $("#search-rows").keyup();
        $('#search-area').addClass('d-none');

        $("#data-row-" + rowID).toggleClass("col-xl-6");
        $("#card-sml-detail-" + rowID).addClass("d-none");
        $("#card-lrg-detail-" + rowID).removeClass("d-none");

    }
}

window.closeProductDetails = closeProductDetails;
function closeProductDetails(rowID) {

    $('#summary-panel-' + rowID).removeClass('d-none');
    $('#search-area').removeClass('d-none');
    $('#expanded-panel-' + rowID).addClass('d-none');
    $('#data-header').removeClass('d-none');
    $("#data-row-" + rowID).toggleClass("col-xl-6");
    $("#search-rows").val('');
    $("#search-rows").keyup();
    $("#card-sml-detail-" + rowID).removeClass("d-none");
    $("#card-lrg-detail-" + rowID).addClass("d-none");
}

var sizeColAmount = 0;

function AddSizeCol(n) {
    sizeColAmount++;
    var $size_type = '                            <div class="size-column" id="sizecol-' + sizeColAmount + '">';
    $size_type += '                              <select class="form-control" id="sizecol-' + sizeColAmount + '-size">';
    $size_type += '                                <option>6</option>';
    $size_type += '                                <option>8</option>';
    $size_type += '                                <option>10</option>';
    $size_type += '                                <option>12</option>';
    $size_type += '                                <option>14</option>';
    $size_type += '                                <option>16</option>';
    $size_type += '                                <option>18</option>';
    $size_type += '                                <option>20</option>';
    $size_type += '                                <option>22</option>';
    $size_type += '                                <option disabled><b>MENS SIZE</b></option>';
    $size_type += '                                <option>XS</option>';
    $size_type += '                                <option>S</option>';
    $size_type += '                                <option>M</option>';
    $size_type += '                                <option>L</option>';
    $size_type += '                                <option>XL</option>';
    $size_type += '                                <option>XXL</option>';
    $size_type += '                                <option disabled><b>EU SIZE</b></option>';
    $size_type += '                                <option>35</option>';
    $size_type += '                                <option>36</option>';
    $size_type += '                                <option>37</option>';
    $size_type += '                                <option>38</option>';
    $size_type += '                                <option>39</option>';
    $size_type += '                                <option>40</option>';
    $size_type += '                                <option>41</option>';
    $size_type += '                                <option>42</option>';
    $size_type += '                              </select>';
    $size_type += '                              <input class="size-input" type="text" value="1"/>';
    $size_type += '                            </div>';


    $($size_type).insertBefore($('.add-size-type'));

}




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


/* -------- admin settings page Start --------- */

function loadNewCheckboxes() {
    var newCategoryCheckboxes = document.querySelectorAll('input[id^=new-cat]');

    // get all settings inputs and watch for keyup event
    for (var i = 0; i < newCategoryCheckboxes.length; i++) {
        newCategoryCheckboxes[i].addEventListener("change", function (event) {


            var j = this.id.lastIndexOf('-');
            var n = this.id.substring(j + 1);

            CategoryCheckChange(n, 1);
        })
    }
}

loadNewCheckboxes();

$(document).ready(function () {
    var allCategoryCheckboxes = document.querySelectorAll('input[id*=existing-cat]');

    // get all settings inputs and watch for keyup event
    for (var i = 0; i < allCategoryCheckboxes.length; i++) {
        allCategoryCheckboxes[i].addEventListener("change", function (event) {


            var j = this.id.lastIndexOf('-');
            var n = this.id.substring(j + 1);

            CategoryCheckChange(n, 0);
        })
    }

});

function CategoryCheckChange(n, i) {

    if (i == 0) {
        var element = document.getElementById("category-line-" + n);
        element.classList.toggle("checked");
    } else {
        var element = document.getElementById("category-new-line-" + n);
        element.classList.toggle("checked");
    }
}

var categoryNumber = 1;

function AddCategoryOLD() {
    const textArea = document.querySelector("#input-category");
    var newCategory = textArea.value;
    var newCategoryCaps = newCategory.substring(0, 1).toUpperCase() +
        newCategory.substring(1, newCategory.length);

    if (newCategory.length > 0) {
        var $category_create = '<div id="category-new-line-' + categoryNumber + '" class="category-line">';
        $category_create += '<label for="new-cat-' + categoryNumber + '"><p>' + newCategoryCaps + '</p><i class="fas fa-times"></i></label>';
        $category_create += '<input type="checkbox" class="category-check" id="new-cat-' + categoryNumber + '">';
        $category_create += '</div>';
        $('#category-select').append($category_create);
        // $( $category_create ).appendTo( '#category-select');
        // document.getElementById("category-select").appendChild(category_create);
        categoryNumber++;

        var element = document.getElementById("category-select");
        element.scrollTop = element.scrollHeight;

        liveAddCategory(newCategory.toString());
    }
}


$('#admin-settings-page .twitter-typeahead').addClass('sticky-top');


/* ---------- admin settings page end ---------------- */

/* --------- product detail page ---------*/
function lowerQuantity() {
    var currentquantity = document.getElementById("quantityvalue").value;
    if (currentquantity > 1) {
        currentquantity--;
    }
    // also sent quantity to the hub
    document.getElementById("quantityvalue").value = currentquantity;
}
function incrementquantity(amount) {
    amount = 5;
    var currentquantity = document.getElementById("quantityvalue").value;
    if (currentquantity < amount) { // the five is replaced with the actual quantity
        currentquantity++;
    }
    // also sent quantity to the hub
    document.getElementById("quantityvalue").value = currentquantity;
}
/* --------- end of product detail page ---------*/

