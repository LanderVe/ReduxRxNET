using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Models;

namespace TestApp.State.Actions
{
  internal class LoadContactsAction { }
  internal class LoadContactsSuccessAction
  {
    private readonly ImmutableList<Contact> data;

    public LoadContactsSuccessAction(ImmutableList<Contact> data)
    {
      this.data = data;
    }
    public ImmutableList<Contact> Data => data;
  }
  internal class LoadContactsFailAction { }


  internal class SaveNewContactAction {
    private readonly Contact contact;

    public SaveNewContactAction(Contact contact)
    {
      this.contact = contact;
    }
    public Contact Contact => contact;
  }
  internal class SaveNewContactSuccessAction
  {
    private readonly Contact contact;

    public SaveNewContactSuccessAction(Contact contact)
    {
      this.contact = contact;
    }
    public Contact Contact => contact;
  }
  internal class SaveNewContactFailAction { }


  internal class UpdateContactAction {
    private readonly Contact contact;

    public UpdateContactAction(Contact contact)
    {
      this.contact = contact;
    }
    public Contact Contact => contact;
  }
  internal class UpdateContactSuccessAction
  {
    private readonly Contact contact;

    public UpdateContactSuccessAction(Contact contact)
    {
      this.contact = contact;
    }
    public Contact Contact => contact;
  }
  internal class UpdateContactFailAction { }


  internal class DeleteContactAction {
    private readonly int contactId;

    public DeleteContactAction(int contactId)
    {
      this.contactId = contactId;
    }
    public int ContactId => contactId;
  }
  internal class DeleteContactSuccessAction
  {
    private readonly int contactId;

    public DeleteContactSuccessAction(int contactId)
    {
      this.contactId = contactId;
    }
    public int ContactId => contactId;
  }
  internal class DeleteContactFailAction { }


}
