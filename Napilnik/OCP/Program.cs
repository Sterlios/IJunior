namespace IMJunior
{
    class Program
    {
        static void Main()
        {
            List<PaymentSystemView> paymentSystemViews = new List<PaymentSystemView>()
            {
                new PaymentSystemView(new PaymentSystemPresenterFactory(new Qiwi())),
                new PaymentSystemView(new PaymentSystemPresenterFactory(new Card())),
                new PaymentSystemView(new PaymentSystemPresenterFactory(new WebMoney())),
            };

            OrderFormPresenterFactory orderFormPresenterFactory = new OrderFormPresenterFactory(paymentSystemViews);
            OrderForm orderForm = new OrderForm(orderFormPresenterFactory);
            OrderCreator orderCreator = new OrderCreator();

            bool isWorking = true;

            while (isWorking)
            {
                Order order = orderCreator.Create();

                orderForm.Open(order);
            }
        }
    }

    public class OrderFormPresenterFactory
    {
        private readonly List<PaymentSystemView> _paymentSystemViews;

        public OrderFormPresenterFactory(List<PaymentSystemView> paymentSystemViews) =>
            _paymentSystemViews = paymentSystemViews
                ?? throw new ArgumentNullException(nameof(paymentSystemViews));

        public OrderFormPresenter Create(OrderForm orderForm) =>
            new OrderFormPresenter(orderForm, _paymentSystemViews);
    }

    public class OrderFormPresenter
    {
        private readonly OrderForm _orderForm;
        private readonly List<PaymentSystemView> _paymentSystemViews;

        public OrderFormPresenter(OrderForm orderForm, List<PaymentSystemView> paymentSystemViews)
        {
            if (paymentSystemViews is null)
                throw new ArgumentNullException(nameof(paymentSystemViews));

            if (paymentSystemViews.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(paymentSystemViews));

            _orderForm = orderForm ?? throw new ArgumentNullException(nameof(orderForm));
            _paymentSystemViews = paymentSystemViews;
        }

        internal void Run(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            _orderForm.ShowMessage("Доступные способы оплаты: ");

            foreach (var paymentSystemView in _paymentSystemViews)
                paymentSystemView.ShowName();

            bool isCorrectSystemId = false;

            while(isCorrectSystemId == false)
            {
                _orderForm.ShowMessage("Какой системой будете оплачивать?");

                string systemId = _orderForm.ReadText();

                foreach (var paymentSystemView in _paymentSystemViews)
                {
                    if (paymentSystemView.Name == systemId)
                    {
                        isCorrectSystemId = true;
                        paymentSystemView.Open(order);
                    }
                }

                if(isCorrectSystemId == false)
                    _orderForm.ShowMessage("Нет такой системы.");
            }
        }
    }

    public class OrderForm
    {
        private readonly OrderFormPresenter _orderFormPresenter;

        public OrderForm(OrderFormPresenterFactory orderFormPresenterFactory)
        {
            if (orderFormPresenterFactory is null)
                throw new ArgumentNullException(nameof(orderFormPresenterFactory));

            _orderFormPresenter = orderFormPresenterFactory.Create(this);
        }

        internal void Open(Order order)
        {
            Console.Clear();
            _orderFormPresenter.Run(order);
            Console.ReadKey();
        }

        internal void ShowMessage(string message) =>
            Console.WriteLine(message);

        internal string ReadText() =>
            Console.ReadLine();
    }

    public class PaymentSystemPresenterFactory
    {
        private readonly PaymentSystem _paymentSystem;

        public PaymentSystemPresenterFactory(PaymentSystem paymentSystem) =>
            _paymentSystem = paymentSystem ?? throw new ArgumentNullException(nameof(paymentSystem));

        public PaymentSystemPresenter Create(PaymentSystemView view) => 
            view is null 
                ? throw new ArgumentNullException(nameof(view)) 
                : new PaymentSystemPresenter(view, _paymentSystem);
    }

    public class PaymentSystemView
    {
        private readonly PaymentSystemPresenter _paymentSystemPresenter;

        public PaymentSystemView(PaymentSystemPresenterFactory paymentSystemPresenterFactory) =>
            _paymentSystemPresenter = paymentSystemPresenterFactory.Create(this);

        public string Name => _paymentSystemPresenter.Name;

        public void Open(Order order) =>
            _paymentSystemPresenter.Pay(order);

        public void ShowName() =>
            Console.WriteLine(Name);

        public void ShowMessage(string message) =>
            Console.WriteLine(message);
    }

    public class PaymentSystemPresenter
    {
        private readonly PaymentSystemView _view;
        private readonly PaymentSystem _paymentSystem;

        public string Name => _paymentSystem.Name;

        public PaymentSystemPresenter(PaymentSystemView view, PaymentSystem paymentSystem)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _paymentSystem = paymentSystem ?? throw new ArgumentNullException(nameof(paymentSystem));
        }

        public void Pay(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            ShowOrderInfo(order);

            _view.ShowMessage($"Переход на страницу оплаты {_paymentSystem.Name}...");
            _paymentSystem.Pay(order);

            ShowOrderInfo(order);
        }

        private void ShowOrderInfo(Order order)
        {
            string status = order.IsBuyed ? "Оплачен" : "Не оплачен";

            _view.ShowMessage($"Заказ: {order.Id} | {status}");
        }
    }

    public abstract class PaymentSystem
    {
        public string Name => GetType().Name;

        public void Pay(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            if (order.IsBuyed)
                throw new InvalidOperationException(nameof(order));

            order.Pay();
        }
    }

    public class Qiwi : PaymentSystem { }

    public class Card : PaymentSystem { }

    public class WebMoney : PaymentSystem { }

    public class Order
    {
        public Order(int id)
        {
            Id = id;
            IsBuyed = false;
        }

        public int Id { get; }
        public bool IsBuyed { get; private set; }

        public void Pay()
        {
            if (IsBuyed)
                throw new InvalidOperationException();

            IsBuyed = true;
        }
    }

    public class OrderCreator
    {
        private int _ids;

        public OrderCreator() =>
            _ids = 0;

        public Order Create()
        {
            _ids++;

            return new Order(_ids);
        }
    }
}