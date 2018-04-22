(function() {
  angular
    .module("primeiraApp")
    .controller("BillingCycleCtrl", ["$http", BillingCycleController]);

  function BillingCycleController($http) {
    const vm = this;
    console.log("Passou...");
    vm.create = function() {
      const url = "http://localhost:3003/api/billingCycles";
      $http.post(url, vm.billingCycle).then(function(response) {
        vm.billingCycle = {};
        console.log("Sucesso!");
      });
    };
  }
})();
