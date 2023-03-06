using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Trainline.PromocodeService.Model;
using Trainline.PromocodeService.Service;
using Trainline.PromocodeService.Service.DiscountStrategies;

namespace Trainline.PromocodeService.Host.UnitTests
{
    public class PromocodePercentageDiscountStrategyTests
    {
        private PromocodePercentageDiscountStrategy _promocodePercentageDiscountStrategy;

        private readonly Discount _discount10Percents = new Discount { Amount = 0.10M, Type = DiscountTypeDefinitions.Percent };

        [SetUp]
        public void Setup()
        {
            _promocodePercentageDiscountStrategy = new PromocodePercentageDiscountStrategy();
        }

        [Test]
        public void DiscountTypeIsAlwaysAmount() => Assert.AreEqual("PERCENT", _promocodePercentageDiscountStrategy.DiscountType);

        [Test]
        public void SingleProduct_AppliesDiscount()
        {
            var productItem = CreateProductItem(110);

            var discountItems = _promocodePercentageDiscountStrategy.GenerateDiscountItems(_discount10Percents, new List<ProductItem> { productItem });

            discountItems.AssertAreEqual(productItem, -11);
        }

        [Test]
        public void MultipleProduct_AppliesDiscount()
        {
            var productItem1 = CreateProductItem(110);
            var productItem2 = CreateProductItem(22);
            var productItem3 = CreateProductItem(3.3M);

            var discountItems = _promocodePercentageDiscountStrategy.GenerateDiscountItems(_discount10Percents, new List<ProductItem> { productItem1, productItem2, productItem3 });

            discountItems.AssertAreEqual(productItem1, -11);
            discountItems.AssertAreEqual(productItem2, -2.2M);
            discountItems.AssertAreEqual(productItem3, -0.33M);
        }


        [Test]
        public void NegativeDiscountValue_Throws()
        {
            var _discount10Percents = new Discount { Amount = -0.01M, Type = DiscountTypeDefinitions.Percent };
            var productItem = CreateProductItem(110);

            Assert.Throws<InvalidOperationException>(() =>
                _promocodePercentageDiscountStrategy
                    .GenerateDiscountItems(_discount10Percents, new List<ProductItem> { productItem })
                    .ToList());
        }


        [Test]
        public void DiscountValueGreaterThenOne_Throws()
        {
            var _discount10Percents = new Discount { Amount = 1.01M, Type = DiscountTypeDefinitions.Percent };
            var productItem = CreateProductItem(110);

            Assert.Throws<InvalidOperationException>(() =>
                _promocodePercentageDiscountStrategy
                    .GenerateDiscountItems(_discount10Percents, new List<ProductItem> { productItem })
                    .ToList());
        }

        [Test]
        public void MultipleProduct_AppliesDiscountWithRounding()
        {
            var productItem1 = CreateProductItem(110.11M);
            var productItem2 = CreateProductItem(22.22M);
            var productItem3 = CreateProductItem(3.3M);
            var discount3Percents = new Discount { Amount = 0.03M, Type = DiscountTypeDefinitions.Percent };
            var discountItems = _promocodePercentageDiscountStrategy.GenerateDiscountItems(discount3Percents, new List<ProductItem> { productItem1, productItem2, productItem3 });

            discountItems.AssertAreEqual(productItem1, -3.3M);
            discountItems.AssertAreEqual(productItem2, -0.67M);
            discountItems.AssertAreEqual(productItem3, -0.10M);
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
}
