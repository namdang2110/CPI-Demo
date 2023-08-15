//Đây là một component được gắn vào mỗi mảnh đối tượng trong trò chơi

using System;
using UnityEngine;
using DG.Tweening;

public class ObjectPiece : MonoBehaviour
{
    //Đối tượng Object đại diện cho tọa độ đúng của mảnh đối tượng
    public GameObject correctForm;
    
    //Hành động Action được gọi khi mảnh đối tượng được đặt đúng vị trí
    public Action OnCorrectPiece;
    
    //Biến moving kiểm tra đối tượng có di chuyển hay không
    private bool moving;
    
    //Thuộc tính đọc-ghi (get - set) dùng để xác đinh xem mảnh đối tượng đã hoàn thành hay chưa
    public bool finish { get; private set; }
    
    //Biến itemCorrect để xác định xem đối tượng đã đặt đúng vị trí hay chưa
    private bool itemCorrect;
    
    //Tọa độ X và Y ban đầu của đối tượng khi di chuyển bắt đầu
    private float startPosX;
    private float startPosY;

    //Vị trí reset ban đầu của mảnh đối tượng
    private Vector3 resetPosition;

    //Hàm Start() được gọi khi đối tượng được khởi tạo
    void Start()
    {
        //Thiết lập vị trí reset ban đầu của đối tượng
        resetPosition = this.transform.localPosition;
    }
    
    //Hàm Update() được gọi mỗi frame của trò chơi
    void Update()
    {
        //Kiểm tra mảnh đối tượng đã hoàn thành 'finish' hay chưa
        if (!finish)
        {
            //Nếu mảnh đối tượng đang di chuyển 'moving', cập nhật vị trí của mảnh đối tượng dựa trên vị trí của chuột 
            if (moving)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY,
                    this.gameObject.transform.localPosition.z);
            }
        }
    }
    
    //Hàm OnMouseDown() được gọi khi chuột được nhấn xuống trên mảnh đối tượng
    private void OnMouseDown()
    {
        //Kiểm tra đối tượng có đúng vị trí 'itemCorrect' và chưa hoàn thành 'finish' hay không
        if (!itemCorrect && !finish && Input.GetMouseButtonDown(0))
        {
            //Nếu chuột trái được nhấn và điều kiện trên đúng, lưu vị trí ban đầu của chuột và đặt moving = true

            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            startPosX = mousePos.x - this.transform.localPosition.x;
            startPosY = mousePos.y - this.transform.localPosition.y;

            moving = true;
            
            SoundManager.Instance.PlayTouchSound();
        }
    }

    //Được gọi khi chuột được nhấc lên khỏi mảnh đối tượng
    private void OnMouseUp()
    {
        //Nếu đối tượng chưa đặt đúng vị trí 'itemCorrect = false' và đang di chuyển 'moving = true'
        if (!itemCorrect && moving)
        {
            moving = false;
            
            //Nếu khoảng cách giữa vị trí của mảnh đối tượng và vị trí đúng 'correctForm' nhỏ hơn hoặc = 0,5
            if (Vector2.Distance(correctForm.transform.position, transform.position) <= 0.5f) 
            {
                //Đồi tượng được di chuyển đến vị trí đúng
                //this.transform.position = new Vector3(correctForm.transform.position.x, correctForm.transform.position.y, correctForm.transform.position.z);
                this.transform.DOLocalMove(new Vector3(correctForm.transform.position.x, correctForm.transform.position.y, correctForm.transform.position.z), 0.5f);
                
                //Gọi hàm CorrectPiece()
                CorrectPiece();
            }
            else
            {
                //Nếu đối tượng chưa được kéo thả đến vị trí đúng, đối tượng sẽ có hoạt ảnh bay về vị trí cũ, sử dụng Dotween
                this.transform.DOLocalMove(resetPosition, 0.5f);
            }
        }
    }

    //Hàm CorrectPiece() được gọi khi mảnh đối tượng đặt đúng vị trí
    public void CorrectPiece()
    {
        //Phát tín hiệu 'đối tượng đã vào đúng vị trí'
        OnCorrectPiece?.Invoke();
        
        //Thay đổi các biến bool
        finish = true;
        itemCorrect = true;
    }
}
