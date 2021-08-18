namespace DefaultNamespace {

    public interface IWallet {
        public bool CanAfford(float amount);
        public void GainMoney(float amount);
        public void SpendMoney(float amount);
        public float GetResellPercentageInDecimal();
    }
}
