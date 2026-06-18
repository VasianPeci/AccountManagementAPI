function getCurrencyCode(currencies, currencyId) {
  return currencies.find((currency) => currency.id === currencyId)?.code || "";
}

export default getCurrencyCode;
