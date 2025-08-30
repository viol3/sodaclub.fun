using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ali.Helper
{
    public class GameUtility
    {
        public static float ClampAngle(float angle, float min, float max)
        {
            angle = Mathf.Repeat(angle, 360);
            min = Mathf.Repeat(min, 360);
            max = Mathf.Repeat(max, 360);
            bool inverse = false;
            var tmin = min;
            var tangle = angle;
            if (min > 180)
            {
                inverse = !inverse;
                tmin -= 180;
            }
            if (angle > 180)
            {
                inverse = !inverse;
                tangle -= 180;
            }
            var result = !inverse ? tangle > tmin : tangle < tmin;
            if (!result)
                angle = min;

            inverse = false;
            tangle = angle;
            var tmax = max;
            if (angle > 180)
            {
                inverse = !inverse;
                tangle -= 180;
            }
            if (max > 180)
            {
                inverse = !inverse;
                tmax -= 180;
            }

            result = !inverse ? tangle < tmax : tangle > tmax;
            if (!result)
                angle = max;
            return angle;
        }
        public static void PrintMatrix(int[,] matrix)
        {
            string result = "";
            for (int i = matrix.GetLength(1) - 1; i >= 0; i--)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    result += matrix[j, i] + " ";
                }
                result += "\r\n";
            }
            Debug.Log(result);
        }

        public static float GetValueFromRatio(float ratio, float min, float max)
        {
            return min + (ratio * (max - min));
        }

        public static float GetRatioFromValue(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        public static Tweener GoToTransform(Transform source, Transform target, float duration, float positionDurationMultiplier = 1f, Ease ease = Ease.Linear)
        {
            source.transform.DOMove(target.position, duration * positionDurationMultiplier).SetEase(ease);
            return source.transform.DORotateQuaternion(target.rotation, duration).SetEase(ease);
        }

        public static void FloodFillMatrix(int[,] mat, int x, int y, int prevV, int newV)
        {
            // Base cases 
            if (x < 0 || x >= mat.GetLength(0) ||
                y < 0 || y >= mat.GetLength(1))
                return;

            if (mat[x, y] != prevV)
                return;

            // Replace the color at (x, y) 
            mat[x, y] = newV;

            // Recur for north, 
            // east, south and west 
            FloodFillMatrix(mat, x + 1, y,
                           prevV, newV);
            FloodFillMatrix(mat, x - 1, y,
                           prevV, newV);
            FloodFillMatrix(mat, x, y + 1,
                           prevV, newV);
            FloodFillMatrix(mat, x, y - 1,
                           prevV, newV);
        }
        public static void Print(string message)
        {
            Debug.Log(message);
        }
		
		public static Texture2D GetCameraTexture(Camera camera)
        {
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            camera.targetTexture = rt;
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();
            camera.targetTexture = null;
            RenderTexture.active = null;
            return texture;
        }

        public static Vector2 GetScreenRatioOfWorldPosition(Vector3 worldPosition)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            Vector2 result = Vector2.zero;
            result.x = screenPos.x / Screen.width;
            result.y = screenPos.y / Screen.height;
            return result;
        }

        public static int GetBSIndexByName(SkinnedMeshRenderer faceMeshRenderer, string bsName)
        {
            Mesh m = faceMeshRenderer.sharedMesh;
            for (int i = 0; i < m.blendShapeCount; i++)
            {
                if (m.GetBlendShapeName(i).Equals(bsName))
                {
                    return i;
                }
            }
            return -1;
        }

        public static float GetAspectRatio()
        {
            return (float)Screen.width / Screen.height;
        }

        public static int[] GetNonRepeatingArray(int start, int count)
        {
            return Enumerable.Range(start, count).OrderBy(X => Random.Range(start, start + count)).ToArray();
        }

        public static Vector2 GetCanvasPositionFromWorldPosition(Vector3 worldPos, RectTransform canvasRect)
        {
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldPos);
            Vector2 worldObjectScreenPos = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

            return worldObjectScreenPos;
        }

        public static Vector3 GetLookAtEulerAngles(Vector3 source, Vector3 target)
        {
            Quaternion lookAtAngle = Quaternion.LookRotation(target - source, Vector3.up);
            return lookAtAngle.eulerAngles;
        }
        public static bool ApproximatelyColor(Color color1, Color color2, int threshold = 1)
        {
            int r1 = (int)(color1.r * 255f);
            int r2 = (int)(color2.r * 255f);
            int g1 = (int)(color1.g * 255f);
            int g2 = (int)(color2.g * 255f);
            int b1 = (int)(color1.b * 255f);
            int b2 = (int)(color2.b * 255f);
            int a1 = (int)(color1.a * 255f);
            int a2 = (int)(color2.a * 255f);
            if (Mathf.Abs(r1 - r2) > threshold)
            {
                return false;
            }
            if (Mathf.Abs(g1 - g2) > threshold)
            {
                return false;
            }
            if (Mathf.Abs(b1 - b2) > threshold)
            {
                return false;
            }
            if (Mathf.Abs(a1 - a2) > threshold)
            {
                return false;
            }
            return true;
        }

        public static KeyCode GetPressedKey()
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(vKey))
                {
                    return vKey;
                }
            }
            return KeyCode.None;
        }

        public static string GetDetailedVector3String(Vector3 vector3)
        {
            return vector3.x + " " + vector3.y + " " + vector3.z;
        }
		
		// Agents keep following until it approaches, can work in a coroutine.
        public static IEnumerator AgentFollowTarget(NavMeshAgent agent, Transform target, float minDistance)
        {
            while (DistanceXZ(target.position, agent.transform.position) > minDistance)
            {
                agent.SetDestination(target.position);
                yield return new WaitForSeconds(0.2f);
            }
        }

        public static float CalculateLengthOfPath(Vector3[] corners)
        {
            float distance = 0f;
            if (corners.Length < 2)
            {
                return 0f;
            }
            for (int i = 0; i < corners.Length - 1; i++)
            {
                distance += Vector3.Distance(corners[i], corners[i + 1]);
            }
            return distance;
        }

        public static float DistanceXZ(Vector3 posA, Vector3 posB)
        {
            posA.y = 0f;
            posB.y = 0f;
            return Vector3.Distance(posA, posB);
        }

        public static float DirectionToAngle(Vector2 dir)
        {
            return Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI;
        }

        public static float GetAngleFromTwoPoints(Vector3 p1, Vector3 p2)
        {
			return Mathf.Atan2(p2.z - p1.z, p2.x - p1.x) * 180 / Mathf.PI;

        }

        public static Vector3 RadianToVector3(float radian)
        {
            return new Vector3(Mathf.Cos(radian), 0f, Mathf.Sin(radian));
        }

        public static Vector3 DegreeToVector3(float degree)
        {
            return RadianToVector3(degree * Mathf.Deg2Rad);
        }
        public static string FormatFloatToReadableString(float value, string format = "", bool isDecimal = false, bool isTwoDigit = false)
        {
            float number = value;
            bool hasDecimalPart = (number % 1) != 0;

            if(number < 100f && isDecimal && hasDecimalPart)
            {
                return number.ToString(isTwoDigit ? "0.0" : "0.00").Replace(",",".");
            }
            if (number < 1000)
            {
                return ((int)number).ToString(format);
            }
            string result = ((int)number).ToString();
            if (result.Contains(","))
            {
                result = result.Substring(0, 4);
                result = result.Replace(",", string.Empty);
            }
            else
            {
                result = result.Substring(0, 3);
            }
            do
            {
                number /= 1000;
            }
            while (number >= 1000);
    
            number = (int)number;
            if (value >= 1000000000000000)
            {
                result = result + "Q";
            }
            else if (value >= 1000000000000)
            {
                result = result + "T";
            }
            else if (value >= 1000000000)
            {
                result = result + "B";
            }
            else if (value >= 1000000)
            {
                result = result + "M";
            }
            else if (value >= 1000)
            {
                result = result + "K";
            }
            if (((int)number).ToString().Length > 0 && ((int)number).ToString().Length < 3)
            {
                result = result.Insert(((int)number).ToString().Length, ".");
            }
            return result;
        }


        public static bool IsStringLetter(string text)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(text.ToUpper());
        }

        public static void Shuffle<T>(ref T[] ts)
        {
            var count = ts.Length;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }

        public static void Shuffle<T>(ref List<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }

        public static void ChangeAlphaSprite(SpriteRenderer sprite, float alpha)
        {
            Color c = sprite.color;
            c.a = alpha;
            sprite.color = c;
        }

        public static void ChangeAlphaImage(Image image, float alpha)
        {
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }

        public static void ChangeAlphaMaskableGraphic(MaskableGraphic image, float alpha)
        {
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }

        public static void ChangeAlphaText(TextMeshProUGUI text, float alpha)
        {
            Color c = text.color;
            c.a = alpha;
            text.color = c;
        }

        public static int RandomIntExcept(int n, params int[] excepts)
        {
            int result = Random.Range(0, n - excepts.Length);

            for (int i = 0; i < excepts.Length; i++)
            {
                if (result < excepts[i])
                    return result;
                result++;
            }
            return result;
        }

        public static int RandomOneOrZero()
        {
            return Random.value > 0.5f ? 1 : 0; ;
        }

        public static int RandomMinusOrPlus()
        {
            return Random.value > 0.5f ? -1 : 1;
        }

        public static bool ArrayContains<T>(T[] array, T element)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (element.Equals(array[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static int[] GetRandomOrderedIntArray(int min, int length)
        {
            int[] resultArray = new int[length];
            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = min + i;
            }
            Shuffle(ref resultArray);
            return resultArray;
        }

        public static Vector3 MouseWorldPosition(float z = 0f)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = Camera.main.transform.position.z;
            pos = Camera.main.ScreenToWorldPoint(pos);
            pos.z = z;
            return pos;
        }

        public static RectTransform GetUIElementOnMouse(GraphicRaycaster gr, string tag)
        {
            //Code to be place in a MonoBehaviour with a GraphicRaycaster component
            //Create the PointerEventData with null for the EventSystem
            PointerEventData ped = new PointerEventData(null);
            //Set required parameters, in this case, mouse position
            ped.position = Input.mousePosition;
            //Create list to receive all results
            List<RaycastResult> results = new List<RaycastResult>();
            //Raycast it
            gr.Raycast(ped, results);
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.tag.Equals(tag))
                {
                    return results[i].gameObject.transform as RectTransform;
                }
            }
            return null;
        }

        public static RectTransform GetUIElementOnMouseByName(GraphicRaycaster gr, string objectName)
        {
            //Code to be place in a MonoBehaviour with a GraphicRaycaster component
            //Create the PointerEventData with null for the EventSystem
            PointerEventData ped = new PointerEventData(null);
            //Set required parameters, in this case, mouse position
            ped.position = Input.mousePosition;
            //Create list to receive all results
            List<RaycastResult> results = new List<RaycastResult>();
            //Raycast it
            gr.Raycast(ped, results);
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.name.Equals(objectName))
                {
                    return results[i].gameObject.transform as RectTransform;
                }
            }
            return null;
        }

        public static T GetNearest<T>(Vector3 source, T[] arrayOfT) where T : MonoBehaviour
        {
            if (arrayOfT.Length == 0)
            {
                return default;
            }
            List<T> sorteds = arrayOfT.OrderBy(x => Vector3.Distance(source, x.transform.position)).ToList();
            return sorteds[0];
        }

        public static T GetFarthest<T>(Vector3 source, T[] arrayOfT) where T : MonoBehaviour
        {
            if (arrayOfT.Length == 0)
            {
                return default;
            }
            List<T> sorteds = arrayOfT.OrderBy(x => Vector3.Distance(source, x.transform.position)).Reverse().ToList();
            return sorteds[Random.Range(0, Mathf.Min(4, arrayOfT.Length))];
        }
    }
}