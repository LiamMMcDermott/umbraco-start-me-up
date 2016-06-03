angular.module("umbraco").controller("KeyValueEditorController", function ($scope, dialogService) {

    $scope.model.value = $scope.model.value || [];

    if (!($scope.model.value instanceof Array))
        $scope.model.value = [];

    $scope.addKeyValue = function () {

        dialogService.open({ template: '/App_Plugins/KeyValueEditor/KeyValueEditorDialogView.html', show: true, callback: done, dialogData: null });

        function done(data) {
            if (data.key && data.value)
                $scope.model.value.push({ key: data.key, value: data.value });
        }

    };

    $scope.editKeyValue = function (item) {

        dialogService.open({ template: '/App_Plugins/KeyValueEditor/KeyValueEditorDialogView.html', show: true, callback: done, dialogData: { key: item.key, value: item.value } });

        function done(data) {
            if (data.key && data.value) {

                for (var i = 0; i < $scope.model.value.length; i++) {
                    if ($scope.model.value[i].key == item.key) {
                        $scope.model.value[i] = data;
                        break;
                    }
                }
            }
        }

    };

    $scope.deleteKeyValue = function (item) {
        $scope.model.value.pop(item);
    };

});