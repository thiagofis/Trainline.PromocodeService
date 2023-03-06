using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.DiscountStrategies;

namespace Trainline.PromocodeService.Host.UnitTests
{
    public class PromocodeAmountDiscountStrategyTests
    {
        private PromocodeAmountDiscountStrategy _promocodeAmountDiscountStrategy;

        private Discount discountOff10 = new Discount { Amount = 10, Type = DiscountTypeDefinitions.Amount };

        [SetUp]
        public void Setup()
        {
            _promocodeAmountDiscountStrategy = new PromocodeAmountDiscountStrategy();
        }

        [Test]
        public void DiscountTypeIsAlwaysAmount() => Assert.AreEqual("AMOUNT", _promocodeAmountDiscountStrategy.DiscountType);

        [Test]
        public void SingleProduct_DiscountBasedAllToThatProduct()
        {
            var productItem = CreateProductItem(100);

            var discountItems = _promocodeAmountDiscountStrategy.GenerateDiscountItems(discountOff10, new List<ProductItem> { productItem });

            discountItems.AssertAreEqual(productItem, -10);
        }
        
        [Test]
        public void TwoProductSameValue_TwoDiscountsSameValue()
        {
            var productItem1 = CreateProductItem(100);
            var productItem2 = CreateProductItem(100);

            var discountItems = _promocodeAmountDiscountStrategy.GenerateDiscountItems(discountOff10, new List<ProductItem> { productItem1, productItem2 });

            discountItems.AssertAreEqual(productItem1, -5);
            discountItems.AssertAreEqual(productItem2, -5);
        }

        [Test]
        public void TwoProductDifferentAmount_TwoDiscountsWithWeightedAmount()
        {
            var productItem1 = CreateProductItem(600);
            var productItem2 = CreateProductItem(400);

            var discountItems = _promocodeAmountDiscountStrategy.GenerateDiscountItems(discountOff10, new List<ProductItem> { productItem1, productItem2 });

            discountItems.AssertAreEqual(productItem1, -6);
            discountItems.AssertAreEqual(productItem2, -4);
        }
        
        [Test]
        public void SingleProductValueLowerThenDiscount_DiscountsAmountOfProductValue()
        {
            var productItem = CreateProductItem(9);

            var discountItems = _promocodeAmountDiscountStrategy.GenerateDiscountItems(discountOff10, new List<ProductItem> { productItem });

            discountItems.AssertAreEqual(productItem, -9);
        }

        [Test]
        public void TwoProductValueLowerThenDiscount_DiscountsAmountOfProductValue()
        {
            var productItem1 = CreateProductItem(1);
            var productItem2 = CreateProductItem(2);

            var discountItems = _promocodeAmountDiscountStrategy.GenerateDiscountItems(discountOff10, new List<ProductItem> { productItem1, productItem2 });
            
            discountItems.AssertAreEqual(productItem1, -1);
            discountItems.AssertAreEqual(productItem2, -2);
        }

        [Test]
        public void ThreeProductValueCousingRoundingIssues_DiscountsAmountRounderProperly()
        {
            var productItem1 = CreateProductItem(100);
            var productItem2 = CreateProductItem(100);
            var productItem3 = CreateProductItem(100);

            var discountItems = _promocodeAmountDiscountStrategy.GenerateDiscountItems(discountOff10, new List<ProductItem> { productItem1, productItem2, productItem3 });

            discountItems.AssertAreEqual(productItem1, -3.34M);
            discountItems.AssertAreEqual(productItem2, -3.33M);
            discountItems.AssertAreEqual(productItem3, -3.33M);
        }

        private ProductItem CreateProductItem(decimal amount)
        {
            var id = Guid.NewGuid().ToString();
            return new ProductItem
            {
                Amount = amount,
                ProductId = id,
                Vendor = Guid.NewGuid().ToString(),
                ProductUri = new Uri("https://product.uri/" + id)
            };
        }
    }

    public static class ProductItemsTestExtensions
    {
        public static void AssertAreEqual(this IEnumerable<ProductItem> discountItems, ProductItem productItem, decimal amount)
        {

            var discountItem = discountItems.Single(x => x.ProductId == productItem.ProductId);
            Assert.AreEqual(productItem.ProductUri, discountItem.ProductUri);
            Assert.AreEqual(productItem.Vendor, discountItem.Vendor);
            Assert.AreEqual(amount, discountItem.Amount);
        }
    }
}
