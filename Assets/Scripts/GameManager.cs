using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	private const float defEachGenerationLastsSeconds = 15f;
	private const float defMinBalanceForce = -50f;
	private const float defMaxBalanceForce = 50f;
	private const float defMinMoveForce = 0f;
	private const float defMaxMoveForce = 25f;
	private const int defPopulationCount = 1000;
	private const float defMutationPercentage = 0.03663f;
	private const float defBodyBalanceFitnessWeight = 0.99f;
	private const float defDistanceTraveledFitnessWeight = 0.01f;
	private const float defDesiredFitness = 0.95f;
	private const int defMaxGenerationCount = 100000;
	private const bool defStopIfReached50MetersIn15Seconds = true;
	private const bool defStopIfReachedDesiredFitness = false;
	private const bool defStopIfReachedMaxGenerationCount = false;


	[SerializeField] private GameObject stickmanPrefab;
	[SerializeField] private CGCCameraFollow followerCamera;
	[SerializeField] private Vector3 startPos;

	private float eachGenerationLastsSeconds;
	private float minBalanceForce;
	private float maxBalanceForce;
	private float minMoveForce;
	private float maxMoveForce;
	private int populationCount;
	private int selectCount;
	private int chromosomeLength;
	private int mutationCount;
	private float bodyBalanceFitnessWeight;
	private float distanceTraveledFitnessWeight;
	private float desiredFitness;
	private int maxGenerationCount;

	private List<bool> stoppingConditionBools;
	/*
	private bool stopIfReached50MetersIn15Seconds;
	private bool stopIfReachedDesiredFitness;
	private bool stopIfReachedMaxGenerationCount;
	*/
	[Space]
	[Header("UI")]
	[SerializeField] private Button settingsButton;
	[SerializeField] private Button runButton;
	[SerializeField] private Sprite[] runButtonSprites;
	[SerializeField] private Text currentGenerationText;
	[SerializeField] private Text bestScoreOfCurrentGenText;
	[SerializeField] private Text bestScoreOfAllGensText;
	[SerializeField] private Text bestReachedOfCurrentGenText;
	[SerializeField] private Text bestReachedOfAllGensText;
	[SerializeField] private Text timeRemainingText;
	[SerializeField] private GameObject parametersScreen;
	[SerializeField] private GameObject closeApplicationConfirmationPanel;

	[Space]
	[Header("Parameters Panel UI")]
	[SerializeField] private InputField eachGenerationLastsSecondsInputField;
	[SerializeField] private InputField minBalanceForceInputField;
	[SerializeField] private InputField maxBalanceForceInputField;
	[SerializeField] private InputField minMoveForceInputField;
	[SerializeField] private InputField maxMoveForceInputField;
	[SerializeField] private InputField populationCountInputField;
	[SerializeField] private InputField mutationPercentageInputField;
	[SerializeField] private InputField bodyBalanceFitnessWeightInputField;
	[SerializeField] private InputField distanceTraveledFitnessWeightInputField;
	[SerializeField] private List<Toggle> stoppingConditionToggles;
	[SerializeField] private InputField desiredFitnessInputField;
	[SerializeField] private InputField maxGenerationCountInputField;

	private float initialTimeRemaining = 15f;
	private float currentRemainingTime;
	public float CurrentRemainingTime
	{
		get => currentRemainingTime;
		set
		{
			if(value < 0)
			{
				currentRemainingTime = 0;
			}
			else
			{
				currentRemainingTime = value;
			}
			timeRemainingText.text = currentRemainingTime.ToString("F1");
		}
	}

	private int currentGeneration;
	public int CurrentGeneration
	{
		get => currentGeneration;
		set
		{
			currentGeneration = value;
			currentGenerationText.text = "Generation: \t\t" + currentGeneration;
		}
	}

	private float bestScoreOfCurrentGen;
	public float BestScoreOfCurrentGen
	{
		get => bestScoreOfCurrentGen;
		set
		{
			bestScoreOfCurrentGen = value;
			bestScoreOfCurrentGenText.text = "Best of This: \t" + bestScoreOfCurrentGen.ToString("F3"); 
		}
	}
	private float bestScoreOfAllGens;
	public float BestScoreOfAllGens
	{
		get => bestScoreOfAllGens;
		set
		{
			bestScoreOfAllGens = value;
			bestScoreOfAllGensText.text = "Best of All: \t\t" + bestScoreOfAllGens.ToString("F3"); 
		}
	}
	
	private float bestReachedOfCurrentGen;
	public float BestReachedOfCurrentGen
	{
		get => bestReachedOfCurrentGen;
		set
		{
			bestReachedOfCurrentGen = value;
			bestReachedOfCurrentGenText.text = "This Reached:\t" + bestReachedOfCurrentGen.ToString("F3"); 
		}
	}
	private float bestReachedOfAllGens;
	public float BestReachedOfAllGens
	{
		get => bestReachedOfAllGens;
		set
		{
			bestReachedOfAllGens = value;
			bestReachedOfAllGensText.text = "Best Reached:\t" + bestReachedOfAllGens.ToString("F3"); 
		}
	}


	List<Individual> population;
	StickManController stickman;

	bool isAlgorithmRunning = false;
	bool shouldStartTimer = false;

	private void Start()
	{
		stoppingConditionBools = new List<bool>();
		for(int i = 0; i < stoppingConditionToggles.Count; ++i)
		{
			stoppingConditionBools.Add(stoppingConditionToggles[i].isOn);
		}
		RestoreParameters();
		StartCoroutine(CreateStickman());
	}

	private void Update()
	{
		if(isAlgorithmRunning == false) { return; }
		if (shouldStartTimer == false) { return; }
		CurrentRemainingTime -= Time.deltaTime;
	}

	#region UI
	public void OnSettingsButton()
	{
		parametersScreen.SetActive(true);
	}

	public void OnCloseSettingsButton()
	{
		parametersScreen.SetActive(false);
	}

	public void OnCloseApplicationButton()
	{
		closeApplicationConfirmationPanel.SetActive(true);
	}

	public void OnConfirmationPanelNoButton()
	{
		closeApplicationConfirmationPanel.SetActive(false);
	}

	public void OnConfirmationPanelYesButton()
	{
		Application.Quit();
	}

	public void OnRestoreDefaultsButton()
	{
		RestoreDefaults();
	}

	public void OnExtraMotivationToggleChange(Toggle toggle)
	{

	}

	public void OnStoppingConditionsToggleValueChanged(Toggle toggle)
	{
		if (!toggle.isOn) { return; }

		int index = stoppingConditionToggles.IndexOf(toggle);

		for(int i = 0; i < stoppingConditionToggles.Count; ++i)
		{
			if(i == index)
			{
				stoppingConditionBools[i] = true;
				if(i == 0)
				{
					desiredFitnessInputField.interactable = false;
					maxGenerationCountInputField.interactable = false;
				}
				else if(i == 1)
				{
					desiredFitnessInputField.interactable = true;
					maxGenerationCountInputField.interactable = false;
				}
				else
				{
					desiredFitnessInputField.interactable = false;
					maxGenerationCountInputField.interactable = true;
				}
				
			}
			else
			{
				stoppingConditionBools[i] = false;
				stoppingConditionToggles[i].isOn = false;
			}
		}
	}

	#endregion

	private void RestoreDefaults()
	{
		RestoreParameters();
		RestoreToggles();
	}

	private void RestoreParameters()
	{
		eachGenerationLastsSecondsInputField.text = defEachGenerationLastsSeconds.ToString();
		minBalanceForceInputField.text = defMinBalanceForce.ToString();
		maxBalanceForceInputField.text = defMaxBalanceForce.ToString();
		minMoveForceInputField.text = defMinMoveForce.ToString();
		maxMoveForceInputField.text = defMaxMoveForce.ToString();
		populationCountInputField.text = defPopulationCount.ToString();
		mutationPercentageInputField.text = defMutationPercentage.ToString();
		bodyBalanceFitnessWeightInputField.text = defBodyBalanceFitnessWeight.ToString();
		distanceTraveledFitnessWeightInputField.text = defDistanceTraveledFitnessWeight.ToString();
		desiredFitnessInputField.text = defDesiredFitness.ToString();
		maxGenerationCountInputField.text = defMaxGenerationCount.ToString();
	}

	private void RestoreToggles()
	{
		stoppingConditionToggles[0].isOn = defStopIfReached50MetersIn15Seconds;
		stoppingConditionToggles[1].isOn = defStopIfReachedDesiredFitness;
		stoppingConditionToggles[2].isOn = defStopIfReachedMaxGenerationCount;

		stoppingConditionBools[0] = defStopIfReached50MetersIn15Seconds;
		stoppingConditionBools[1] = defStopIfReachedDesiredFitness;
		stoppingConditionBools[2] = defStopIfReachedMaxGenerationCount;
	}

	private void SetParameters()
	{
		eachGenerationLastsSeconds = float.Parse(eachGenerationLastsSecondsInputField.text);
		minBalanceForce = int.Parse(minBalanceForceInputField.text);
		maxBalanceForce = int.Parse(maxBalanceForceInputField.text);
		minMoveForce = int.Parse(minMoveForceInputField.text);
		maxMoveForce = int.Parse(maxMoveForceInputField.text);
		populationCount = int.Parse(populationCountInputField.text);
		selectCount = (int) Mathf.Sqrt(populationCount);
		bodyBalanceFitnessWeight = float.Parse(bodyBalanceFitnessWeightInputField.text);
		distanceTraveledFitnessWeight = float.Parse(distanceTraveledFitnessWeightInputField.text);
		desiredFitness = float.Parse(desiredFitnessInputField.text);
		maxGenerationCount = int.Parse(maxGenerationCountInputField.text);

		initialTimeRemaining = eachGenerationLastsSeconds;
		int removeChromosomeCount = (int)(initialTimeRemaining / 15f);
		chromosomeLength = (int) (initialTimeRemaining / 0.25f) - removeChromosomeCount;
		mutationCount = (int)(float.Parse(mutationPercentageInputField.text) * chromosomeLength);
	}

	public void OnRunButton()
	{
		if (!isAlgorithmRunning)
		{
			SetParameters();
			ResetStickman();
			StartRunning();
		}
		else
		{
			StopRunning();
		}
	}

	private void StartRunning()
	{
		settingsButton.interactable = false;
		BestScoreOfAllGens = 0;
		BestScoreOfCurrentGen = 0;
		BestReachedOfCurrentGen = 0;
		BestReachedOfAllGens = 0f;
		CurrentGeneration = 1;
		CurrentRemainingTime = initialTimeRemaining;
		followerCamera.Player = stickman.transform.GetChild(2).gameObject;
		StartCoroutine(InitFirstGeneration());
		runButton.GetComponent<Image>().sprite = runButtonSprites[1];
		isAlgorithmRunning = true;
	}

	private void StopRunning()
	{
		StopAllCoroutines();
		runButton.GetComponent<Image>().sprite = runButtonSprites[0];
		isAlgorithmRunning = false;
		settingsButton.interactable = true;
	} 

	private IEnumerator CreateStickman()
	{
		yield return new WaitForSeconds(0.25f);
		stickman = Instantiate(stickmanPrefab, startPos, Quaternion.identity).GetComponent<StickManController>();
		Physics2D.IgnoreLayerCollision(6, 6);
		yield return null;
	}

	private void ResetStickman()
	{
		stickman.Individual = null;
		Transform[] childTransforms = stickman.GetComponentsInChildren<Transform>();
		for(int j = 0; j < childTransforms.Length; ++j)
		{
			childTransforms[j].position = stickman.childLocations[j];
			childTransforms[j].rotation = stickman.childRotations[j];
		}
		stickman.gameObject.SetActive(true);
	}

	private IEnumerator InitFirstGeneration()
	{
		CurrentGeneration = 1;
		population = new List<Individual>();

		for (int i = 0; i < populationCount; ++i)
		{
			population.Add(GenerateRandomChromosome());
			//stickmen[i].Individual = population[i];
		}
		yield return StartCoroutine(CalculateFitnesses(population));
		shouldStartTimer = true;
		yield return StartCoroutine(ShowBestIndividual(population));
		yield return StartCoroutine(GeneticAlgorithm());
		StopRunning();
	}

	private IEnumerator GeneticAlgorithm()
	{
		while (!ShouldStop())
		{
			shouldStartTimer = false;
			CurrentRemainingTime = initialTimeRemaining;
			CurrentGeneration++;
			//DestroyStickmen();
			//CreateStickmen();
			ResetStickman();
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


			yield return StartCoroutine(CalculateFitnesses(population));
			shouldStartTimer = true;
			yield return StartCoroutine(ShowBestIndividual(population));
		}
		yield return null;
	}

	private bool ShouldStop()
	{
		if (stoppingConditionBools[2])
		{
			//Debug.LogError("maxgen");
			return CurrentGeneration >= maxGenerationCount;
		}
		else if (stoppingConditionBools[1])
		{
			//Debug.LogError("desfit");
			return BestScoreOfAllGens >= desiredFitness;
		}
		else
		{
			//Debug.LogError("reached50");
			return BestScoreOfAllGens >= 1f;
		}
	}

	private IEnumerator ShowBestIndividual(List<Individual> population)
	{
		sort(population, 0, population.Count - 1);
		Individual best = population[population.Count - 1];
		stickman.Individual = best;

		for (int i = 0; i < StickManController.musclesCount; ++i)
		{
			stickman.muscles[i].force = stickman.Individual.forces[i];
		}

		for (int i = 0; i < chromosomeLength; ++i)
		{
			yield return StartCoroutine(best.Move(best.chromosome[i]));
			BestReachedOfCurrentGen = best.stickman.playerHipJumpScript.transform.position.x;
			BestScoreOfCurrentGen = (best.stickman.playerHipJumpScript.transform.position.x / 50f);
			//moveBackgroundMasterScript.MoveAll(best.stickman.playerHipJumpScript.transform.position.x);
			if (BestScoreOfCurrentGen > BestScoreOfAllGens)
			{
				BestScoreOfAllGens = BestScoreOfCurrentGen;
			}
			if(BestReachedOfCurrentGen > BestReachedOfAllGens)
			{
				BestReachedOfAllGens = BestReachedOfCurrentGen;
			}
		}
		//Debug.LogError(after - before);
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
		individual.fitness = 0;
		for(int i = 0; i < StickManController.musclesCount; ++i)
		{
			individual.fitness += individual.forces[i];
		}

		individual.fitness /= StickManController.musclesCount;
		individual.fitness /= maxBalanceForce;
		individual.fitness *= bodyBalanceFitnessWeight;
		
		for(int i = 0; i < chromosomeLength; ++i)
		{
			/*for (int j = 0; j < StickManController.musclesCount; ++j)
			{
				individual.stickman.muscles[j].force = individual.chromosome[i].forces[j];
			}*/
			//yield return StartCoroutine(individual.Move(individual.chromosome[i]));
			individual.fitness += individual.chromosome[i].addForceVector.x * distanceTraveledFitnessWeight;
		}
		individual.isFitnessCalculationFinished = true;
		yield return null;
	}

	private List<Individual> SelectIndividuals(List<Individual> population)
	{
		List<Individual> sortedPopulation = new List<Individual>();
		List<Individual> newPopulation = new List<Individual>();

		sortedPopulation.AddRange(population);
		sort(sortedPopulation, 0, sortedPopulation.Count - 1);

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

		Individual newIndividual = new Individual(chromosomeLength, startPos);

		newIndividual.chromosome.AddRange(individual1.chromosome.GetRange(0, newLengthChromosome_1));
		newIndividual.chromosome.AddRange(individual2.chromosome.GetRange(newLengthChromosome_1, newLengthChromosome_2));
		newIndividual.chromosome.AddRange(individual1.chromosome.GetRange(newLengthChromosome_2, newLengthChromosome_3));

		if(individual1.forces[0] > individual2.forces[0])
		{
			newIndividual.forces.AddRange(individual1.forces);
		}
		else
		{
			newIndividual.forces.AddRange(individual2.forces);
		}

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
		Individual individual = new Individual(chromosomeLength, startPos);

		//individual.stickman = stickman;

		for (int i = 0; i < chromosomeLength; ++i)
		{
			Gene gene = GenerateRandomGene(i % 2);
			individual.chromosome.Add(gene);
		}

		float randomForce = Random.Range(minBalanceForce, maxBalanceForce);
		for (int j = 0; j < StickManController.musclesCount; ++j)
		{
			individual.forces.Add(randomForce);
		}
		return individual;
	}


	public Gene GenerateRandomGene(int muscleIndex)
	{
		//muscleIndex = Random.Range(0, StickManController.movableMusclesCount);
		float rotationX = Random.Range(minMoveForce, maxMoveForce);
		float rotationY = Random.Range(0f, 0f);
		float moveRotationRotation = Random.Range(0, 120f);
		/*if(muscleIndex == 0)
		{
			rotationX *= -1;
			moveRotationRotation *= -1;
		}*/
		float moveRotationLerpMultiplier = Random.Range(300, 600f);

		Vector2 addForceVector = new Vector2(rotationX, rotationY);

		Gene gene = new Gene(muscleIndex, /*new Vector2(10,0), 60, 1000 */addForceVector, moveRotationRotation, moveRotationLerpMultiplier);
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
		public List<float> forces;


		public Individual(int length, Vector3 startPos)
		{
			this.chromosomeLength = length;
			chromosome = new List<Gene>(length);
			forces = new List<float>();
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
		//public Vector2 rotation;
		//public float multiplierForce;
		public Gene(int muscleIndex, Vector2 addForceVector, float moveRotationRotation, float moveRotationLerpMultiplier)
		{
			this.muscleIndex = muscleIndex;
			this.addForceVector = addForceVector;
			this.moveRotationRotation = moveRotationRotation;
			this.moveRotationLerpMultiplier = moveRotationLerpMultiplier;
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