// See https://aka.ms/new-console-template for more information
using MathNet.Numerics.LinearAlgebra;



namespace ForReal
{
    class Program
    {
        public static int InputNumbers = 3;
        public static int OutputNumbers = 3;
        public static int HiddenNumbers = 3;
        public static int Population_Size = 100;
        public static int Offspring_Length = InputNumbers*HiddenNumbers+HiddenNumbers*OutputNumbers;
        static float[] Pop_Fitness(float[] Input_Data, float[,] Populations){
            float[] Fitness = new float[Population_Size];
            var M = Matrix<double>.Build;
            for(int i = 0; i<Population_Size; i++){
                float ans = 0;
                var MIH = M.Dense(HiddenNumbers, InputNumbers, (x, y) => Populations[i, x+y*HiddenNumbers]);
                var MHO = M.Dense(OutputNumbers, HiddenNumbers, (x, y) => Populations[i, InputNumbers*HiddenNumbers+x+y*OutputNumbers]);
                var InputMat = M.Dense(InputNumbers, 1, (x, y)=>Input_Data[x]);
                var OutputMat = MHO * MHO * InputMat;
                for(int j = 0; j<OutputNumbers; j++){
                    ans += Convert.ToSingle(OutputMat[j, 0]);
                }
                Fitness[i]=ans;
            }
            return Fitness;
        }
        static float[,] Sel_Mating_Pool(float[] Fitness, float[,] Populations, int Num_Parents){
            float[,] Mating_Pool = new float[Num_Parents, Offspring_Length];
            for(int i = 0; i<Num_Parents; i++){
                float mini = Fitness[0];
                int minij=0;
                for(int j = 0; j<Population_Size; j++){
                    if(mini>Fitness[j]){
                        mini = Fitness[j];
                        minij = j;
                    }
                }
                for(int j = 0; j<Offspring_Length; j++){
                    Mating_Pool[i,j]=Populations[minij, j];
                }
                Fitness[minij] = float.PositiveInfinity;
            }
            return Mating_Pool;
        }
        static float[,] Crossover(float[,] Mating_Pool, int Offspring_num, int Num_Parents){
            float[,] Offsprings = new float[Offspring_num, Offspring_Length];
            int crossover_point1 = Offspring_Length/3;
            int crossover_point2 = Offspring_Length/3*2;
            for(int i = 0; i<Offspring_num; i++){
                int Mater1 = i%Num_Parents;
                int Mater2 = (i+1)%Num_Parents;
                for(int j = 0; j<crossover_point1; j++){
                    Offsprings[i,j] = Mating_Pool[Mater1, j];
                }
                for(int j = crossover_point1; j<crossover_point2; j++){
                    Offsprings[i,j] = Mating_Pool[Mater2, j];
                }
                for(int j = crossover_point2; j<Offspring_Length; j++){
                    Offsprings[i,j] = Mating_Pool[Mater1, j];
                }
            }
            return Offsprings;
        }
        static float[,] Mutation(float[,] Offsprings, int Offspring_num){
            var rand = new Random();
            for(int i = 0; i<Offspring_num; i++){
                double x = rand.NextDouble()*2;  //0% ~ 200%의 변이
                float y = Convert.ToSingle(x);
                Offsprings[i, rand.Next(0, Offspring_Length)] *= y;
            }
            return Offsprings;
        }
        static void Main(string[] args)
        {
            float[] inputData = new float[InputNumbers];
            for(int i = 0; i<InputNumbers; i++){
                inputData[i]=1;
            }
            float[,] newPop = new float[Population_Size, Offspring_Length];
            var rand = new Random();
            for(int i = 0; i<Population_Size; i++){
                for(int j = 0; j<Offspring_Length; j++){
                    newPop[i,j]=Convert.ToSingle(rand.NextDouble());
                }
            }
            int num_Gene = 100;
            for(int i = 0; i<num_Gene; i++){
                Console.WriteLine(i);
                float[] Fitness = Pop_Fitness(inputData, newPop);
                for(int j = 0; j<Population_Size; j++){
                    Console.WriteLine(Fitness[j]);
                }
                float[,] Maters = Sel_Mating_Pool(Fitness, newPop, 30);
                float[,] Offsprings = Crossover(Maters, 70, 30);
                float[,] MutOffsprings = Mutation(Offsprings, 70);
                for(int j = 0; j<30; j++){
                    for(int k = 0; k<Offspring_Length; k++){
                        newPop[j, k]=Maters[j, k];
                    }
                }
                for(int j = 0; j<70; j++){
                    for(int k = 0; k<Offspring_Length; k++){
                        newPop[j+30, k]=Offsprings[j, k];
                    }
                }
            }
        }
    }
}
