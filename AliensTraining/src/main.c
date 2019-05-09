#include <stdio.h>
#include <stdlib.h>
#include <time.h>

const int    SPECIES_COUNT              = 6;
const int    GENERATIONS_TO_TRAIN       = 5000;
const int    POPULATION_SIZE            = 250;
const int    ALIENS_PER_ROW             = 6;
const double TOTAL_GAME_TIME            = 120.0;
const double CROSSOVER_RATE             = 0.7;
const double MUTATION_RATE              = 0.01;
const double AVERAGE_FITNESS_PERCENTAGE = 0.01;

const char*  SAVE_FILENAME        = "aliens.txt";

typedef struct
{
    int     actionsCount;
    double* timesToShoot;
} Gene;

typedef struct
{
    Gene*  genes;
    double fitness;
} Genome;

typedef struct
{
    Genome* population;
} Specie;

double RandomDouble(double min, double max)
{
    double range = (max - min); 
    double div = RAND_MAX / range;
    return min + (rand() / div);
}

void RandomGene(Gene* gene, int row)
{
    gene->actionsCount = (SPECIES_COUNT - row) + 1;
    gene->timesToShoot = (double*)malloc(sizeof(double) * gene->actionsCount);

    for(int i = 0; i < gene->actionsCount; i++)
    {
        gene->timesToShoot[i] = RandomDouble(0.0, TOTAL_GAME_TIME);
    }
}

void CreateInitialGenome(Genome* genome, int row)
{
    genome->fitness = 0.0;
    genome->genes   = (Gene*)malloc(sizeof(Gene) * ALIENS_PER_ROW);

    for(int j = 0; j < ALIENS_PER_ROW; j++)
    {
        RandomGene(&(genome->genes[j]), row);
    }
}

void CreateInitialPopulation(Specie* specie, int row)
{
    specie->population = (Genome*)malloc(sizeof(Genome) * POPULATION_SIZE);

    for(int i = 0; i < POPULATION_SIZE; i++)
    {
        CreateInitialGenome(&(specie->population[i]), row);
    }
}

void QuickSort(double* times, int left, int right)
{
    int    i = left;
    int    j = right;
    double m = times[(left + right) / 2];
    double tmp;

    while(i <= j)
    {
        while(times[i] < m)
            i++;
        
        while(times[j] > m)
            j--;

        if(i <= j)
        {
            tmp = times[i];
            times[i] = times[j];
            times[j] = tmp;

            i++;
            j--;
        }
    }

    if(left < j)
        QuickSort(times, left, j);
    if(i < right)
        QuickSort(times, i, right);
}

double CalculateIndividualFintness(Gene* individual)
{
    double  result = 0.0;
    int     numTimes = individual[0].actionsCount * ALIENS_PER_ROW;
    double* allTimes = (double*)malloc(sizeof(double) * numTimes); 
    int     index = 0;
    double  average = 0.0;

    for(int i = 0; i < ALIENS_PER_ROW; i++)
    {
        for(int j = 0; j < individual[i].actionsCount; j++)
        {
            allTimes[index++] = individual[i].timesToShoot[j];
        }
    }

    QuickSort(allTimes, 0, index-1);
    result = allTimes[0];

    for(int i = 0; i < index; i++)
    {
        double next = TOTAL_GAME_TIME;
        if(i < index - 1)
            next = allTimes[i + 1];
        
        double difference = next - allTimes[i];
        average += allTimes[i];

        if(difference < result)
            result = difference;
    }

    average /= index;

    result += (average * AVERAGE_FITNESS_PERCENTAGE);

    free(allTimes);
    allTimes = NULL;

    return result;
}

void CalculatePopulationFitness(Genome* population)
{
    for(int i = 0; i < POPULATION_SIZE; i++)
        population[i].fitness = CalculateIndividualFintness(population[i].genes);
}

void CalculateTotalFitness(Genome* population, double* totalFitness)
{
    *totalFitness = 0.0;

    for(int i = 0; i < POPULATION_SIZE; i++)
        *totalFitness += population[i].fitness;
}

void DeletePopulation(Genome** population)
{
    for(int i = 0; i < POPULATION_SIZE; i++)
    {
        for(int j = 0; j < ALIENS_PER_ROW; j++)
        {
            free((*population)[i].genes[j].timesToShoot);
            (*population)[i].genes[j].timesToShoot;
         }
        free((*population)[i].genes);
        (*population)[i].genes = NULL;
    }
    free(*population);
    *population = NULL;
}

Genome* RouletteWheelSelection(Genome* population, double totalFitness)
{
    double slice       = RandomDouble(0.0, totalFitness);
    double accumulated = 0.0;
    int    returnIndex = 0;

    for(int i = 0; i < POPULATION_SIZE; i++)
    {
        accumulated += population[i].fitness;
        if(accumulated >= slice)
        {
            returnIndex = i;
            i = POPULATION_SIZE + 1; // break
        }
    }

    return &population[returnIndex];
}

void CrossoverMultiPoint(const Genome* mum, const Genome* dad, Genome* baby1, Genome* baby2)
{
    for(int i = 0; i < ALIENS_PER_ROW; i++)
    {
        for(int j = 0; j < mum->genes[i].actionsCount; j++)
        {
            if(RandomDouble(0.0, 1.0) < CROSSOVER_RATE)
            {
                baby1->genes[i].timesToShoot[j] = dad->genes[i].timesToShoot[j];
                baby2->genes[i].timesToShoot[j] = mum->genes[i].timesToShoot[j];
            }
            else
            {
                baby1->genes[i].timesToShoot[j] = mum->genes[i].timesToShoot[j];
                baby2->genes[i].timesToShoot[j] = dad->genes[i].timesToShoot[j];
            }
        }
    }
}

void Mutate(Genome* baby)
{
    for(int i = 0; i < ALIENS_PER_ROW; i++)
    {
        for(int j = 0; j < baby->genes[i].actionsCount; j++)
        {
            if(RandomDouble(0.0, 1.0) < MUTATION_RATE)
            {
                double increase = RandomDouble(0.0, TOTAL_GAME_TIME * 2.0);
                increase -= TOTAL_GAME_TIME;

                baby->genes[i].timesToShoot[j] += increase;
                if(baby->genes[i].timesToShoot[j] < 0.0)
                    baby->genes[i].timesToShoot[j] = 0.0;
                
                if(baby->genes[i].timesToShoot[j] > TOTAL_GAME_TIME)
                    baby->genes[i].timesToShoot[j] = TOTAL_GAME_TIME;
            }
        }
    }
}

void CopyBest(Genome* newPopulation, int* addIndex, Genome* prevPopulation, int row)
{
    int bestIndex = 0;
    
    for(int i = 0; i < POPULATION_SIZE; i++)
        if(prevPopulation[i].fitness > prevPopulation[bestIndex].fitness)
            bestIndex = i;
    
    Genome chosenGenome = prevPopulation[bestIndex];
    Genome newGenome;

    CreateInitialGenome(&newGenome, row);

    for(int i = 0; i < ALIENS_PER_ROW; i++)
    {
        for(int j = 0; j < chosenGenome.genes[i].actionsCount; j++)
        {
            newGenome.genes[i].timesToShoot[j] = chosenGenome.genes[i].timesToShoot[j];
        }
    }

    // if(row == 5)
        // printf("%lf\n", chosenGenome.fitness);

    newPopulation[(*addIndex)++] = newGenome;
}

void Epoch(Specie* specie, int row)
{
    CalculatePopulationFitness(specie->population);
    double totalFitness = 0.0;
    CalculateTotalFitness(specie->population, &totalFitness);

    Genome* newPopulation = (Genome*)malloc(sizeof(Genome) * POPULATION_SIZE);

    int addIndex = 0;

    CopyBest(newPopulation, &addIndex, specie->population, row);
    CopyBest(newPopulation, &addIndex, specie->population, row);

    while(addIndex < POPULATION_SIZE)
    {
        Genome* mum = RouletteWheelSelection(specie->population, totalFitness);
        Genome* dad = RouletteWheelSelection(specie->population, totalFitness);

        Genome baby1;
        Genome baby2;
        CreateInitialGenome(&baby1, row);
        CreateInitialGenome(&baby2, row);

        CrossoverMultiPoint(mum, dad, &baby1, &baby2);

        Mutate(&baby1);
        Mutate(&baby2);

        // add the babies to the population
        newPopulation[addIndex++] = baby1;
        newPopulation[addIndex++] = baby2;
    }

    DeletePopulation(&specie->population);
    specie->population = newPopulation;
}

int main(void)
{
    srand(time(NULL));
    Specie* species = (Specie*)malloc(sizeof(Specie) * SPECIES_COUNT);
    FILE* fout = fopen(SAVE_FILENAME, "w");

    for(int i = 0; i < SPECIES_COUNT; i++)
        CreateInitialPopulation(&species[i], i);

    for(int i = 0; i < GENERATIONS_TO_TRAIN; i++)
    {
        for(int j = 0; j < SPECIES_COUNT; j++)
            Epoch(&species[j], j);
    }

    fprintf(fout, "%d %d\n", SPECIES_COUNT, ALIENS_PER_ROW);
    
    for(int i = 0; i < SPECIES_COUNT; i++)
    {
        for(int j = 0; j < 1; j++)
        {
            for(int k = 0; k < ALIENS_PER_ROW; k++)
            {
                QuickSort(species[i].population[j].genes[k].timesToShoot, 0, species[i].population[j].genes[k].actionsCount - 1);
                
                fprintf(fout, "%d ", species[i].population[j].genes[k].actionsCount);
                for(int l = 0; l < species[i].population[j].genes[k].actionsCount; l++)
                {
                    fprintf(fout, "%lf ", species[i].population[j].genes[k].timesToShoot[l]);
                }
                fprintf(fout, "\n");
            }
        }
    }

    // Free memory
    for(int i = 0; i < SPECIES_COUNT; i++)
    {
        DeletePopulation(&species[i].population);
    }

    fclose(fout);
    fout = NULL;

    free(species);
    species = NULL;

    return 0;
}