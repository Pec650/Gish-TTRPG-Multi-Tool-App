namespace MauiApp1;

public class User
{
    private int _id { get; set; }
    private String _fname { get; set; }
    private String _lname { get; set; }
    private String _email { get; set; }

    public int id
    {
        get { return _id; }
        set { _id = value; }
    }
    
    public String fname
    {
        get { return _fname; }
        set { _fname = value; }
    }
    
    public String lname
    {
        get { return _lname; }
        set { _lname = value; }
    }

    public String email
    {
        get { return _email; }
        set { _email = value; }
    }

    public User()
    {
        emptyUser();
    }

    public void emptyUser()
    {
        id = -1;
        fname = string.Empty;
        lname = string.Empty;
        email = string.Empty;
    }

    public bool userExists()
    {
        return (id != -1);
    }

    public void setCurrentUser(int id, string fname, string lname, string email)
    {
        this.id = id;
        this.fname = fname;
        this.lname = lname;
        this.email = email;
    }
}