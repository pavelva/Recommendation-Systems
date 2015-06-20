using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniProject.Utils
{
    class RandomUniqeGen<T>
    {
        private Random randomGen;
        private List<T> elements;
        private int iterations;

        public RandomUniqeGen(List<T> elements)
        {
            this.randomGen = new Random();
            this.elements = elements;
            this.iterations = elements.Count;
        }

        public T next()
        {
            if (iterations == 0)
            {
                this.iterations = elements.Count;
                return elements[0];
            }
            int random = randomGen.Next(0, iterations);
            T result = elements[random];
            iterations--;
            elements[random] = elements[iterations];
            elements[iterations] = result;
            return result;
        }
    }
}
