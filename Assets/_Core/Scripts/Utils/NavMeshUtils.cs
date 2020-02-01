using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
	public static NavMeshPath CalculatePathToTarget(this NavMeshAgent agent, Vector3 target)
	{
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(target, path);
		return path;
	}

	public static float CalculateLengthPathToTarget(this NavMeshAgent agent, Vector3 target)
	{
		NavMeshPath path = CalculatePathToTarget(agent, target);
		float lengthPath = 0f;

		if (path.corners.Length > 0)
		{
			lengthPath += Vector3.Distance(agent.transform.position, path.corners[0]);
			lengthPath += Vector3.Distance(path.corners[0], target);
		}

		if (path.corners.Length > 1 && path.status != NavMeshPathStatus.PathInvalid)
		{
			for (int i = 0; i < path.corners.Length - 2; i++)
			{
				Vector3 currentCorner = path.corners[i];
				Vector3 nextCorner = path.corners[i + 1];
				lengthPath += Vector3.Distance(currentCorner, nextCorner);
			}
		}
		return lengthPath;
	}
}
