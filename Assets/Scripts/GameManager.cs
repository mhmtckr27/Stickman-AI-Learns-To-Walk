using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameObject stickmanPrefab;
	[SerializeField] private const float maxRotationAngle = 10;
	private const float minBoneForce = 0f;
	private const float maxBoneForce = 30f;
	private const float minForceRotationX = 0f;
	private const float maxForceRotationX = 20f;
	[SerializeField] private Vector3 startPos;
	[SerializeField] private int chromosomeLength = 1000;
	[SerializeField] private int populationCount = 1000;
	[SerializeField] private int selectCount = 30;
	[SerializeField] private int generationCount = 100;
	[SerializeField] private int mutationCount = 3;
	[SerializeField] private CGCCameraFollow followerCamera;
	[SerializeField] private Text currentGenerationText;
	[SerializeField] private Text bestScoreOfCurrentGenText;
	[SerializeField] private Text bestScoreOfAllGensText;

	private int currentGeneration;
	public int CurrentGeneration
	{
		get => currentGeneration;
		set
		{
			currentGeneration = value;
			currentGenerationText.text = "Generation: " + currentGeneration;
		}
	}

	private float bestScoreOfCurrentGen;
	public float BestScoreOfCurrentGen
	{
		get => bestScoreOfCurrentGen;
		set
		{
			bestScoreOfCurrentGen = value;
			bestScoreOfCurrentGenText.text = "Best Score of Current Gen: \t" + bestScoreOfCurrentGen; 
		}
	}
	private float bestScoreOfAllGens;
	public float BestScoreOfAllGens
	{
		get => bestScoreOfAllGens;
		set
		{
			bestScoreOfAllGens = value;
			bestScoreOfAllGensText.text = "Best Score of All Gens: \t\t" + bestScoreOfAllGens; 
		}
	}


	List<Individual> population;
	List<StickManController> stickmen;

	bool isAlgorithmRunning = false;


	private void Start()
	{
	}

	private void Update()
	{
		if(isAlgorithmRunning == false) { return; }
		foreach(Individual individual in population)
		{
			foreach (SpriteRenderer spriteRenderer in individual.stickman.GetComponentsInChildren<SpriteRenderer>())
			{
				Color color = spriteRenderer.color;
				color = Color.white;
				color.a = 0.1f;
				spriteRenderer.color = color;
			}
		}
	}

	private void LateUpdate()
	{
		if(isAlgorithmRunning == false) { return; }
		sort(population, 0, population.Count - 1);
		foreach(SpriteRenderer spriteRenderer in population[population.Count - 1].stickman.GetComponentsInChildren<SpriteRenderer>())
		{
			Color color = spriteRenderer.color;
			color = Color.green;
			color.a = 1;
			spriteRenderer.color = color;
			followerCamera.Player = population[population.Count - 1].stickman.transform.GetChild(2).gameObject;
			BestScoreOfCurrentGen = population[population.Count - 1].fitness;
			if(BestScoreOfCurrentGen > BestScoreOfAllGens)
			{
				BestScoreOfAllGens = BestScoreOfCurrentGen;
			}
			//spriteRenderer.enabled = true;
		}
	}

	public void OnRunButton()
	{
		StartCoroutine(CreateStickmen());
		StartCoroutine(InitFirstGeneration());
	}

	private void DestroyStickmen()
	{
		for (int i = 0; i < stickmen.Count; i++)
		{
			Destroy(stickmen[i].gameObject);
		}
	}

	private IEnumerator CreateStickmen()
	{
		stickmen = new List<StickManController>();
		/*for (int i = 0; i < populationCount; ++i)
		{*/
			StartCoroutine(CreateStickmanRoutine());
		//}

		Physics2D.IgnoreLayerCollision(6, 6);
		yield return null;
	}

	IEnumerator CreateStickmanRoutine()
	{
		stickmen.Add(Instantiate(stickmanPrefab, startPos, Quaternion.identity).GetComponent<StickManController>());
		yield return null;
	}

	private void ResetStickmen()
	{
		for(int i = 0; i < populationCount; ++i)
		{
			stickmen[i].Individual = null;
			Transform[] childTransforms = stickmen[i].GetComponentsInChildren<Transform>();
			for(int j = 0; j < childTransforms.Length; ++j)
			{
				childTransforms[j].position = stickmen[i].childLocations[j];
				childTransforms[j].rotation = stickmen[i].childRotations[j];
			}
		}
		for(int i = 0; i < populationCount; ++i)
		{
			stickmen[i].gameObject.SetActive(true);
		}
	}

	private IEnumerator InitFirstGeneration()
	{
		CurrentGeneration = 1;
		population = new List<Individual>();

		for (int i = 0; i < populationCount; ++i)
		{
			population.Add(GenerateRandomChromosome());
			stickmen[i].Individual = population[i];
		}
		isAlgorithmRunning = true;
		yield return StartCoroutine(CalculateFitnesses(population));
		yield return StartCoroutine(GeneticAlgorithm());
	}

	private IEnumerator GeneticAlgorithm()
	{
		Debug.Log("Population Count: " + population.Count);
		while (CurrentGeneration < generationCount)
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
				//stickmen[i].Individual = population[i];
			}

			for(int i = 0; i < population.Count; i++)
			{
				MutateChromosome(population[i]);
			}

			for(int i = 0; i < population.Count; ++i)
			{
				stickmen[i].Individual = population[i];
			}

			yield return StartCoroutine(CalculateFitnesses(population));
			CurrentGeneration++;
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
			/*for (int j = 0; j < StickManController.musclesCount; ++j)
			{
				individual.stickman.muscles[j].force = individual.chromosome[i].forces[j];
			}*/
			if (individual.isDead)
			{
				break;
			}
			yield return StartCoroutine(individual.Move(individual.chromosome[i]));
			individual.fitness += individual.chromosome[i].addForceVector.x;
		}
		Debug.LogError(individual.fitness);
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

		newIndividual.chromosome.AddRange(individual1.chromosome.GetRange(0, newLengthChromosome_1));
		newIndividual.chromosome.AddRange(individual2.chromosome.GetRange(newLengthChromosome_1, newLengthChromosome_2));
		newIndividual.chromosome.AddRange(individual1.chromosome.GetRange(newLengthChromosome_2, newLengthChromosome_3));

		return newIndividual;
	}

	private void MutateChromosome(Individual individual)
	{
		for (int i = 0; i < mutationCount; ++i)
		{
			individual.chromosome[Random.Range(0, individual.chromosome.Count)] = GenerateRandomGene(Random.Range(0, 2));
		}
	}

	private Individual GenerateRandomChromosome()
	{
		Individual individual = new Individual(chromosomeLength, stickmanPrefab, startPos);

		//individual.stickman = stickman;

		for (int i = 0; i < chromosomeLength; ++i)
		{
			Gene gene = GenerateRandomGene(i % 2);
			individual.chromosome.Add(gene);
		}

		return individual;
	}


	public Gene GenerateRandomGene(int muscleIndex)
	{
		//muscleIndex = Random.Range(0, StickManController.movableMusclesCount);
		float rotationX = Random.Range(maxForceRotationX, maxForceRotationX);
		float rotationY = Random.Range(0f, 0f);
		float moveRotationRotation = Random.Range(0, 120f);
		/*if(muscleIndex == 0)
		{
			//rotationX *= -1;
			moveRotationRotation *= -1;
		}*/
		float moveRotationLerpMultiplier = Random.Range(300, 600f);

		Vector2 addForceVector = new Vector2(rotationX, rotationY);

		Gene gene = new Gene(muscleIndex, /*new Vector2(10,0), 60, 1000 */addForceVector, moveRotationRotation, moveRotationLerpMultiplier);
		for (int j = 0; j < StickManController.musclesCount; ++j)
		{
			gene.forces.Add(Random.Range(minBoneForce, maxBoneForce));
		}
		return gene;
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
			yield return new WaitForSeconds(0.25f);
			//stickman.Balance();
			stickman.Step2(gene.muscleIndex, gene.addForceVector, gene.moveRotationRotation, gene.moveRotationLerpMultiplier);
			//stickman.Balance();

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