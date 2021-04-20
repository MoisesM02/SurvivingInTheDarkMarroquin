using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class PlayerController : MonoBehaviour
{

    public FACE_DIRECTION direction = FACE_DIRECTION.RIGHT;//
    public static PlayerController player = null;//Variable estatica que hace referencia a asi misma

    public bool canJump = true;//Puedo saltar
    public bool canMove = true;//Puedo mover al personaje
    public bool isOnTheGround = false;

    public float jumpPower = 600;//Le aplicamos fuerza para que salte
    public float jumpTimeout = 1.0f; //tiempo entre el salto
    public float maxSpeed = 40.0f; //velocidad maxima del personaje

    //Los tipos de movimientos asignados
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";// no la usaremos
    public string jumpButton = "Jump";

    private Rigidbody2D theRigidbody = null;
    private Transform theTransform = null;//posicion
    public BoxCollider2D feetCollider = null;//boxcollider de los pies
    public LayerMask groundLayer; //objetos con los que puede colisionar


    public enum FACE_DIRECTION//concepto de enumerado, clase que puede tomar 2 valores
    {
        LEFT = -1,//vemos a la izquierda
        RIGHT = 1//vemos a la derecha
    };

     public static float Health {//la vida del personaje
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                Die();
            }
        }
    }

    [SerializeField]
    private static float _health = 100.0f;

    void Awake(){
        theRigidbody = GetComponent<Rigidbody2D>();//despertamos los Rigibody
        theTransform = GetComponent<Transform>();//despertamos los Rigibody
        player = this;
    }

    //*************************Salto***************************************
    private void Jump(){//metodo para saltar
        if (!isOnTheGround || !canJump) //si no esta en el suelo o no puede saltar
            return;

        theRigidbody.AddForce(Vector2.up * jumpPower);//fuerza de salto
        canJump = false;//si ya esta saltando no puede saltar
        Invoke("EnableJump", jumpTimeout);//invocamos metodo para activar salto.
    }
    private void EnableJump(){//metodo que activa salto
        canJump = true;
    }

    //*************************El el suelo ********************************
    private bool GetGrounded(){//metodo para aterrrizar en el suelo
        Vector2 boxCenter = new Vector2(theTransform.position.x, theTransform.position.y) + feetCollider.offset;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(boxCenter, feetCollider.size, 0, groundLayer);

        if (hitColliders.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //*************************Cambiar direccion del personaje*************
    private void ChangeDirection(){//metodo para girar el personaje
        direction = (FACE_DIRECTION)((int)direction * -1.0f);
        Vector3 localScale = theTransform.localScale;
        localScale.x *= -1.0f;
        theTransform.localScale = localScale;
    }

    void FixedUpdate(){ //este metodo nos ayuda a controlar sobre que le pasa al personaje
        if (!canMove || Health <= 0)
            return;


        isOnTheGround = GetGrounded();

        float horizontal = CrossPlatformInputManager.GetAxis(horizontalAxis);
        theRigidbody.AddForce(Vector2.right * horizontal * maxSpeed);


        if ((horizontal < 0 && direction != FACE_DIRECTION.LEFT) || (horizontal > 0 && direction != FACE_DIRECTION.RIGHT))
            ChangeDirection();



        if (CrossPlatformInputManager.GetButton(jumpButton))
        {
            Jump();
        }


        theRigidbody.velocity = new Vector2(
            Mathf.Clamp(theRigidbody.velocity.x, -maxSpeed, maxSpeed),//
            Mathf.Clamp(theRigidbody.velocity.y, -Mathf.Infinity, jumpPower)
        );
    }

     void OnDestroy()
    {
        player = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static void Die()
    {
        Destroy(PlayerController.player.gameObject);
    }

    public static void Reset()//revivir el personaje
    {
        Health = 100.0f;
    }
}
