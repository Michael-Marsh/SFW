﻿using SFW.Helpers;
using System.Windows.Controls;
using System.Windows.Input;

namespace SFW.UserLogIn
{
    public class ViewModel : ViewModelBase
    {
        #region Properties

        public string UserName { get; set; }
        public string NewPwd { get; set; }
        public string ConfirmPwd { get; set; }
        public string OldPwd { get; set; }


        private bool _viewType;
        public bool ViewType
        {
            get
            {
                return _viewType;
            }
            set
            {
                _viewType = value;
                OnPropertyChanged(nameof(ViewType));
            }
        }

        private string _error;
        public string Error

        {
            get
            { return _error; }
            set
            {
                _error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        public RelayCommand _loginCommand;

        public RelayCommand _passResetCommand;

        #endregion

        /// <summary>
        /// Log In ViewModel Default Constructor
        /// </summary>
        public ViewModel()
        {
            UserName = string.Empty;
            ViewType = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDiff"></param>
        public ViewModel(bool vType)
        {
            ViewType = vType;
        }




        #region Log In ICommand

        public ICommand LogInCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand(LogInExecute, LogInCanExecute);
                }
                return _loginCommand;
            }
        }

        /// <summary>
        /// Log In ICommand Execution
        /// </summary>
        /// <param name="parameter">Will contain a secure password object</param>
        public void LogInExecute(object parameter)
        {
            if (ViewType)
            {
                if (parameter != null && parameter.GetType() == typeof(PasswordBox[]))
                {
                    Error = CurrentUser.UpdatePassword(UserName, ((PasswordBox[])parameter)[0].Password, ((PasswordBox[])parameter)[1].Password);
                    if (string.IsNullOrEmpty(Error))
                    {


                        foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
                        {
                            if (w.Title == "Password Reset")
                            {
                                w.Close();
                                if ((ShopRoute.ViewModel)Controls.WorkSpaceDock.WccoDock.GetChildOfType<ShopRoute.View>().DataContext != null)
                                {
                                    ((ShopRoute.ViewModel)Controls.WorkSpaceDock.WccoDock.GetChildOfType<ShopRoute.View>().DataContext).UpdateView();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var _result = CurrentUser.LogIn(UserName, ((PasswordBox)parameter).Password);
                if (!_result.ContainsKey(0) && _result.TryGetValue(1, out string s))
                {
                    Error = s;
                    //TODO: add in the viewmodel change representation
                    ViewType = true;
                    //CurrentUser.UpdatePassword(UserName, ((PasswordBox)parameter).Password, NewPwd);
                }
                else if (!_result.ContainsKey(0))
                {
                    foreach (var v in _result)
                    {
                        Error = v.Value;
                    }
                }
                else
                {
                    if (CurrentUser.IsLoggedIn)
                    {
                        foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
                        {
                            if (w.Title == "User Log In")
                            {
                                w.Close();
                                if ((ShopRoute.ViewModel)Controls.WorkSpaceDock.WccoDock.GetChildOfType<ShopRoute.View>().DataContext != null)
                                {
                                    ((ShopRoute.ViewModel)Controls.WorkSpaceDock.WccoDock.GetChildOfType<ShopRoute.View>().DataContext).UpdateView();
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool LogInCanExecute(object parameter)
        {
            if (ViewType)
            {
                if (parameter != null && parameter.GetType() == typeof(PasswordBox[]))
                {

                    return !string.IsNullOrEmpty(((PasswordBox[])parameter)[0].Password)
                        && !string.IsNullOrEmpty(((PasswordBox[])parameter)[1].Password)
                        && !string.IsNullOrEmpty(((PasswordBox[])parameter)[2].Password)
                        && ((PasswordBox[])parameter)[1].Password == ((PasswordBox[])parameter)[2].Password
                        && ((PasswordBox[])parameter)[0].Password != ((PasswordBox[])parameter)[1].Password
                        && !string.IsNullOrEmpty(UserName)
                        && CurrentUser.UserExist(UserName);
                }
                return false;
            }
            else
            {
                return !string.IsNullOrEmpty(UserName) || !string.IsNullOrEmpty(((PasswordBox)parameter).Password);
            }
        }

        #endregion

        /// <summary>
        /// Object disposal
        /// </summary>
        /// <param name="disposing">Called by the GC Finalizer</param>
        public override void OnDispose(bool disposing)
        {
            if (disposing)
            {               
                
            }
        }
    }
}
