$(function(){
    function loadOrders(status){
        $.getJSON('/admin/order/list/' + status, function(res){
            if(!res.isSuccess){
                alert(res.message || 'Error');
                return;
            }
            var rows = '';
            $.each(res.data, function(i, o){
                rows += '<tr>' +
                    '<td>'+o.id+'</td>' +
                    '<td>'+o.userId+'</td>' +
                    '<td>'+o.totalAmount+'</td>' +
                    '<td>'+ (o.trackingId||'') +'</td>' +
                    '<td>'+o.status+'</td>' +
                    '<td>'+o.createdDate+'</td>' +
                    '<td><button class="btn btn-sm btn-primary add-tracking" data-id="'+o.id+'">Add/Update Tracking</button></td>' +
                    '</tr>';
            });
            $('#orders-table tbody').html(rows);
        });
    }

    $('body').on('click', '.add-tracking', function(){
        var id = $(this).data('id');
        $('#trackingModal input[name=orderId]').val(id);
        $('#trackingModal').modal('show');
    });

    $('#saveTracking').click(function(){
        var dto = {
            orderId: parseInt($('#trackingModal input[name=orderId]').val()),
            trackingId: $('#trackingId').val(),
            status: $('#status').val(),
            updatedBy: parseInt($('#adminUserId').val() || 0)
        };
        $.ajax({
            url: '/admin/order/update-tracking',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function(res){
                if(res.isSuccess){
                    $('#trackingModal').modal('hide');
                    loadOrders($('#tabs .active').data('status'));
                    toastr.success(res.message || 'Updated');
                } else {
                    toastr.error(res.message || 'Failed');
                }
            }
        });
    });

    $('#tabs a[data-status]').click(function(e){
        e.preventDefault();
        $('#tabs a').removeClass('active');
        $(this).addClass('active');
        loadOrders($(this).data('status'));
    });

    // initial load pending
    $('a[data-status="Pending"]').click();
});
