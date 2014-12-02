using System;
using System.Text;
using NSpec;

namespace uMigrations.Tests.features.MovePropertyUp
{
    /* uncomment Console.Write in after_all to see the example. 
     * due to bug in nspec after_all is called even when spec is excluded from execution by tag filter
     */

    [Tag("example")]
    internal class describe_Execution_Order_Example : feature
    {
        private static StringBuilder _buffer = new StringBuilder();

        private void before_all()
        {
            _buffer.AppendLine("before_all");
        }

        private void after_all()
        {
            _buffer.AppendLine("after_all");
             // Console.Write(_buffer.ToString());
        }

        private void before_each()
        {
            _buffer.AppendLine("before_each");
        }

        private void after_each()
        {
            _buffer.AppendLine("after_each");
        }

        private void describe_Something()
        {
            context["With something"] = () =>
            {
                beforeAll = () => _buffer.AppendLine("describe_Something->With something beforeAll");
                beforeEach = () => _buffer.AppendLine("describe_Something->With something beforeEach");
                afterAll = () => _buffer.AppendLine("describe_Something->With something afterAll");
                afterEach = () => _buffer.AppendLine("describe_Something->With something afterEach");

                context["when nested something"] = () =>
                {
                    beforeAll =
                        () =>
                            _buffer.AppendLine("describe_Something->With something->when nested something beforeAll");
                    beforeEach =
                        () =>
                            _buffer.AppendLine(
                                "describe_Something->With something->when nested something beforeEach");
                    afterAll =
                        () =>
                            _buffer.AppendLine("describe_Something->With something->when nested something afterAll");
                    afterEach =
                        () =>
                            _buffer.AppendLine("describe_Something->With something->when nested something afterEach");

                    it["first it"] = () =>
                    {
                        act = () => _buffer.AppendLine("first it->act()");

                        _buffer.AppendLine("first it");
                    };

                    it["second it"] = () =>
                    {
                        act = () => _buffer.AppendLine("second it");

                        _buffer.AppendLine("second it");
                    };
                };
            };
        }

        private void describe_Another_Something()
        {
            context["With something"] = () =>
            {
                beforeAll = () => _buffer.AppendLine("describe_Something->With something beforeAll");
                beforeEach = () => _buffer.AppendLine("describe_Something->With something beforeEach");
                afterAll = () => _buffer.AppendLine("describe_Something->With something afterAll");
                afterEach = () => _buffer.AppendLine("describe_Something->With something afterEach");

                context["when nested something"] = () =>
                {
                    beforeAll =
                        () =>
                            _buffer.AppendLine("describe_Something->With something->when nested something beforeAll");
                    beforeEach =
                        () =>
                            _buffer.AppendLine(
                                "describe_Something->With something->when nested something beforeEach");
                    afterAll =
                        () =>
                            _buffer.AppendLine("describe_Something->With something->when nested something afterAll");
                    afterEach =
                        () =>
                            _buffer.AppendLine("describe_Something->With something->when nested something afterEach");

                    it["first it"] = () =>
                    {
                        act = () => _buffer.AppendLine("first it->act()");

                        _buffer.AppendLine("first it");
                    };

                    it["second it"] = () =>
                    {
                        act = () => _buffer.AppendLine("second it");

                        _buffer.AppendLine("second it");
                    };
                };
            };
        }

        class nested_spec : nspec
        {
            private void before_all()
            {
                _buffer.AppendLine("nested_spec->before_all");
            }

            private void after_all()
            {
                _buffer.AppendLine("nested_spec->after_all");
            }

            private void before_each()
            {
                _buffer.AppendLine("nested_spec->before_each");
            }

            private void after_each()
            {
                _buffer.AppendLine("nested_spec->after_each");
            }

            private void describe_Something()
            {
                context["nested_spec->With something"] = () =>
                {
                    beforeAll = () => _buffer.AppendLine("nested_spec->describe_Something->With something beforeAll");
                    beforeEach = () => _buffer.AppendLine("nested_spec->describe_Something->With something beforeEach");
                    afterAll = () => _buffer.AppendLine("nested_spec->describe_Something->With something afterAll");
                    afterEach = () => _buffer.AppendLine("nested_spec->describe_Something->With something afterEach");

                    context["nested_spec->when nested something"] = () =>
                    {
                        beforeAll =
                            () =>
                                _buffer.AppendLine("nested_spec->describe_Something->With something->when nested something beforeAll");
                        beforeEach =
                            () =>
                                _buffer.AppendLine(
                                    "nested_spec->describe_Something->With something->when nested something beforeEach");
                        afterAll =
                            () =>
                                _buffer.AppendLine("nested_spec->describe_Something->With something->when nested something afterAll");
                        afterEach =
                            () =>
                                _buffer.AppendLine("nested_spec->describe_Something->With something->when nested something afterEach");

                        it["nested_spec->first it"] = () =>
                        {
                            act = () => _buffer.AppendLine("nested_spec->first it->act()");

                            _buffer.AppendLine("nested_spec->first it");
                        };

                        it["nested_spec->second it"] = () =>
                        {
                            act = () => _buffer.AppendLine("nested_spec->second it");

                            _buffer.AppendLine("nested_spec->second it");
                        };
                    };
                };
            }
        }

        private void describe_Yet_Another_Something()
        {
            context["With something"] = () =>
            {
                beforeAll = () => _buffer.AppendLine("describe_Something->With something beforeAll");
                beforeEach = () => _buffer.AppendLine("describe_Something->With something beforeEach");
                afterAll = () => _buffer.AppendLine("describe_Something->With something afterAll");
                afterEach = () => _buffer.AppendLine("describe_Something->With something afterEach");

                context["when nested something"] = () =>
                {
                    beforeAll =
                        () =>
                            _buffer.AppendLine("describe_Something->With something->when nested something beforeAll");
                    beforeEach =
                        () =>
                            _buffer.AppendLine(
                                "describe_Something->With something->when nested something beforeEach");
                    afterAll =
                        () =>
                            _buffer.AppendLine("describe_Something->With something->when nested something afterAll");
                    afterEach =
                        () =>
                            _buffer.AppendLine("describe_Something->With something->when nested something afterEach");

                    it["first it"] = () =>
                    {
                        act = () => _buffer.AppendLine("first it->act()");

                        _buffer.AppendLine("first it");
                    };

                    it["second it"] = () =>
                    {
                        act = () => _buffer.AppendLine("second it");

                        _buffer.AppendLine("second it");
                    };
                };
            };
        }
    }
}