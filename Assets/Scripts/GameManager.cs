using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameObject stickmanPrefab;
	[SerializeField] private const float maxRotationAngle = 10;
	private const int minBoneForce = 0;
	private const float maxBoneForce = 40f;
	[SerializeField] private Vector3 startPos;
	[SerializeField] private int chromosomeLength = 1000;
	[SerializeField] private int populationCount = 1000;
	[SerializeField] private int selectCount = 30;
	[SerializeField] private int generationCount = 100;
	[SerializeField] private int mutationCount = 3;

	public int currentGeneration;
	List<Individual> population;
	List<StickManController> stickmen;

	private void Start()
	{
	}

	public void OnRunButton()
	{
		CreateStickmen();
		StartCoroutine(InitFirstGeneration());
	}

	private void DestroyStickmen()
	{
		for (int i = 0; i < stickmen.Count; i++)
		{
			Destroy(stickmen[i].gameObject);
		}
	}

	private void CreateStickmen()
	{
		stickmen = new List<StickManController>();
		for (int i = 0; i < populationCount; ++i)
		{
			stickmen.Add(Instantiate(stickmanPrefab, startPos, Quaternion.identity).GetComponent<StickManController>());
		}

		for (int i = 0; i < stickmen.Count; ++i)
		{
			Collider2D[] colliders1 = stickmen[i].GetComponentsInChildren<Collider2D>();
			for (int j = i + 1; j < stickmen.Count; ++j)
			{
				Collider2D[] colliders2 = stickmen[j].GetComponentsInChildren<Collider2D>();
				StartCoroutine(IgnoreCollisionRoutine(colliders1, colliders2));
			}
		}
	}

	IEnumerator IgnoreCollisionRoutine(Collider2D[] colliders1, Collider2D[] colliders2)
	{
		for (int k = 0; k < colliders1.Length; ++k)
		{
			for (int m = 0; m < colliders2.Length; ++m)
			{
				Physics2D.IgnoreCollision(colliders1[k], colliders2[m]);
			}
		}
		yield return null;
	}

	private void ResetStickmen()
	{
		for(int i = 0; i < populationCount; ++i)
		{
			stickmen[i].transform.position = startPos;
			stickmen[i].transform.rotation = Quaternion.identity;
			stickmen[i].Individual = null;
			for(int j = 0; j < stickmen[i].transform.childCount; ++j)
			{
				stickmen[i].transform.GetChild(j).localPosition = stickmen[i].childLocations[j];
				stickmen[i].transform.GetChild(j).localRotation = stickmen[i].childRotations[j];
			}
		}
	}

	private IEnumerator InitFirstGeneration()
	{
		currentGeneration = 1;
		population = new List<Individual>();

		for (int i = 0; i < populationCount; ++i)
		{
			population.Add(GenerateRandomChromosome());
			stickmen[i].Individual = population[i];
			for (int j = 0; j < StickManController.musclesCount; ++j)
			{
				stickmen[i].muscles[j].force = population[i].forces[j];
			}
		}
		yield return StartCoroutine(CalculateFitnesses(population));
		yield return StartCoroutine(GeneticAlgorithm());
	}

	private IEnumerator GeneticAlgorithm()
	{
		Debug.Log("Population Count: " + population.Count);
		while (currentGeneration < generationCount)
		{
			Debug.Log("Population Count: " + population.Count);
			//DestroyStickmen();
			//CreateStickmen();
			ResetStickmen();
			population = SelectIndividuals(population);
			population = ReproducePopulation(population);

			for(int i = population.Count; i < populationCount; ++i)
			{
				population.Add(GenerateRandomChromosome());
				stickmen[i].Individual = population[i];
				for (int j = 0; j < StickManController.musclesCount; ++j)
				{
					stickmen[i].muscles[j].force = population[i].forces[j];
				}
			}

			for(int i = 0; i < population.Count; i++)
			{
				MutateChromosome(population[i]);
			}

			for(int i = 0; i < population.Count; ++i)
			{
				stickmen[i].Individual = population[i];
				for(int j = 0; j < StickManController.musclesCount; ++j)
				{
					stickmen[i].muscles[j].force = population[i].forces[j];
				}
			}

			yield return StartCoroutine(CalculateFitnesses(population));
			currentGeneration++;
		}
		yield return null;
	}

	private IEnumerator CalculateFitnesses(List<Individual> population)
	{
		foreach(Individual individual in population)
		{
			StartCoroutine(CalculateFitness(individual));
		}
		bool shouldExit = false;
		while(shouldExit == false)
		{
			yield return new WaitForSeconds(0.1f);
			shouldExit = true;
			foreach(Individual individual in population)
			{
				if(individual.isFitnessCalculationFinished == false)
				{
					shouldExit = false;
				}
			}
		}
		yield return null;
	}


	private IEnumerator CalculateFitness(Individual individual)
	{
		for(int i = 0; i < chromosomeLength; ++i)
		{
			if (individual.isDead)
			{
				break;
			}
			yield return StartCoroutine(individual.Move(individual.chromosome[i]));
		}
		individual.fitness = individual.stickman.transform.GetChild(0).position.x;
		individual.isFitnessCalculationFinished = true;
		if (individual.isDead)
		{
			individual.stickman.gameObject.SetActive(false);
		}
	}

	private List<Individual> SelectIndividuals(List<Individual> population)
	{
		List<Individual> sortedPopulation = new List<Individual>();
		List<Individual> newPopulation = new List<Individual>();

		sortedPopulation.AddRange(population);
		sort(sortedPopulation, 0, sortedPopulation.Count - 1);

		Debug.LogError(sortedPopulation[sortedPopulation.Count - 1].fitness);

		for (int i = 0; i < selectCount; ++i)
		{
			newPopulation.Add(sortedPopulation[sortedPopulation.Count - 1 - i]);
		}

		return newPopulation;
	}

	private List<Individual> ReproducePopulation(List<Individual> populationToReproduce)
	{
		List<Individual> newPopulation = new List<Individual>();

		for(int i = 0; i < populationToReproduce.Count; ++i)
		{
			for(int j = 0; j < populationToReproduce.Count; ++j)
			{
				if(i!= j)
				{
					newPopulation.Add(ReproduceChromosome(populationToReproduce[i], populationToReproduce[j]));
				}
			}
		}

		return newPopulation;
	}

	private Individual ReproduceChromosome(Individual individual1, Individual individual2)
	{
		int newLengthChromosome_1 = Random.Range(0, 1) * chromosomeLength;
		int newLengthChromosome_2 = Random.Range(0, 1) * (chromosomeLength - newLengthChromosome_1);
		int newLengthChromosome_3 = individual1.chromosome.Count - (newLengthChromosome_1 + newLengthChromosome_2);

		Individual newIndividual = new Individual(chromosomeLength, stickmanPrefab, startPos);

		for (int i = 0; i < StickManController.musclesCount; ++i)
		{
			newIndividual.forces.Add(Random.Range(minBoneForce, maxBoneForce));
		}

		newIndividual.chromosome.AddRange(individual1.chromosome.GetRange(0, newLengthChromosome_1));
		newIndividual.chromosome.AddRange(individual2.chromosome.GetRange(newLengthChromosome_1, newLengthChromosome_2));
		newIndividual.chromosome.AddRange(individual1.chromosome.GetRange(newLengthChromosome_2, newLengthChromosome_3));

		return newIndividual;
	}

	private void MutateChromosome(Individual individual)
	{
		for (int i = 0; i < mutationCount; ++i)
		{
			individual.chromosome[Random.Range(0, individual.chromosome.Count)] = GenerateRandomGene(individual);
		}
	}

	private Individual GenerateRandomChromosome()
	{
		Individual individual = new Individual(chromosomeLength, stickmanPrefab, startPos);

		//individual.stickman = stickman;

		for(int i = 0; i < StickManController.musclesCount; ++i)
		{
			individual.forces.Add(Random.Range(minBoneForce, maxBoneForce));
		}

		for (int i = 0; i < chromosomeLength; ++i)
		{
			Gene gene = GenerateRandomGene(individual);
			
			individual.chromosome.Add(gene);
		}

		return individual;
	}


	public Gene GenerateRandomGene(Individual individual)
	{
		int muscleIndex = Random.Range(0, StickManController.movableMusclesCount);
		float rotationX = Random.Range(0f, 10f);
		//float rotationY = Random.Range(0f, 60f);
		float moveRotationRotation = Random.Range(0, 30f);
		/*if(muscleIndex == 0)
		{
			rotationX *= -1;
			moveRotationRotation *= -1;
		}*/
		float moveRotationLerpMultiplier = Random.Range(100, 300f);

		Vector2 addForceVector = new Vector2(rotationX, 0 /*rotationY*/);

		return new Gene(muscleIndex, /*new Vector2(10,0), 60, 1000 */addForceVector, moveRotationRotation, moveRotationLerpMultiplier);
	}



	public class Individual
	{
		int chromosomeLength;
		public StickManController stickman;
		public List<Gene> chromosome;
		public float fitness;
		public bool isFitnessCalculationFinished;
		public bool isDead;


		public Individual(int length, GameObject stickmanPrefab, Vector3 startPos)
		{
			this.chromosomeLength = length;
			chromosome = new List<Gene>(length);
		}

		public IEnumerator Move(Gene gene)
		{
			yield return new WaitForSeconds(0.2f);
			stickman.Step2(gene.muscleIndex, gene.addForceVector, gene.moveRotationRotation, gene.moveRotationLerpMultiplier);
		}

	}

	public class Gene
	{
		public int muscleIndex;
		public Vector2 addForceVector;
		public float moveRotationRotation;
		public float moveRotationLerpMultiplier;
		public List<float> forces;
		//public Vector2 rotation;
		//public float multiplierForce;
		public Gene(int muscleIndex, Vector2 addForceVector, float moveRotationRotation, float moveRotationLerpMultiplier)
		{
			this.muscleIndex = muscleIndex;
			this.addForceVector = addForceVector;
			this.moveRotationRotation = moveRotationRotation;
			this.moveRotationLerpMultiplier = moveRotationLerpMultiplier;
			forces = new List<float>();
		}
	}


	#region Merge Sort

	// Merges two subarrays of []arr.
	// First subarray is arr[l..m]
	// Second subarray is arr[m+1..r]
	void merge(List<Individual> arr, int l, int m, int r)
	{
		// Find sizes of two
		// subarrays to be merged
		int n1 = m - l + 1;
		int n2 = r - m;

		// Create temp arrays
		Individual[] L = new Individual[n1];
		Individual[] R = new Individual[n2];
		int i, j;

		// Copy data to temp arrays
		for (i = 0; i < n1; ++i)
			L[i] = arr[l + i];
		for (j = 0; j < n2; ++j)
			R[j] = arr[m + 1 + j];

		// Merge the temp arrays

		// Initial indexes of first
		// and second subarrays
		i = 0;
		j = 0;

		// Initial index of merged
		// subarry array
		int k = l;
		while (i < n1 && j < n2)
		{
			if (L[i].fitness <= R[j].fitness)
			{
				arr[k] = L[i];
				i++;
			}
			else
			{
				arr[k] = R[j];
				j++;
			}
			k++;
		}

		// Copy remaining elements
		// of L[] if any
		while (i < n1)
		{
			arr[k] = L[i];
			i++;
			k++;
		}

		// Copy remaining elements
		// of R[] if any
		while (j < n2)
		{
			arr[k] = R[j];
			j++;
			k++;
		}
	}

	// Main function that
	// sorts arr[l..r] using
	// merge()
	void sort(List<Individual> arr, int l, int r)
	{
		if (l < r)
		{
			// Find the middle
			// point
			int m = l + (r - l) / 2;

			// Sort first and
			// second halves
			sort(arr, l, m);
			sort(arr, m + 1, r);

			// Merge the sorted halves
			merge(arr, l, m, r);
		}
	}

	// A utility function to
	// print array of size n */
	static void printArray(List<Individual> arr)
	{
		int n = arr.Count;
		for (int i = 0; i < n; ++i)
			Debug.Log(arr[i] + " ");
		Debug.Log("");
	}
	#endregion
}