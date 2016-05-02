﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.18034.
// 
#pragma warning disable 1591

namespace ServiceEngine.TFRechargeService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ServiceSoap", Namespace="http://tempuri.org/")]
    public partial class Service : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback RechargeOperationCompleted;
        
        private System.Threading.SendOrPostCallback checkWalletOperationCompleted;
        
        private System.Threading.SendOrPostCallback getTransactionStatusOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Service() {
            this.Url = global::ServiceEngine.Properties.Settings.Default.ServiceEngine_TFRechargeService_Service;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event RechargeCompletedEventHandler RechargeCompleted;
        
        /// <remarks/>
        public event checkWalletCompletedEventHandler checkWalletCompleted;
        
        /// <remarks/>
        public event getTransactionStatusCompletedEventHandler getTransactionStatusCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Recharge", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Recharge(string OpCode, string custMobNo, string rchAmt, string clientMobileNo, string Password, string operatorType, string rechargeType, string agentTransId) {
            object[] results = this.Invoke("Recharge", new object[] {
                        OpCode,
                        custMobNo,
                        rchAmt,
                        clientMobileNo,
                        Password,
                        operatorType,
                        rechargeType,
                        agentTransId});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void RechargeAsync(string OpCode, string custMobNo, string rchAmt, string clientMobileNo, string Password, string operatorType, string rechargeType, string agentTransId) {
            this.RechargeAsync(OpCode, custMobNo, rchAmt, clientMobileNo, Password, operatorType, rechargeType, agentTransId, null);
        }
        
        /// <remarks/>
        public void RechargeAsync(string OpCode, string custMobNo, string rchAmt, string clientMobileNo, string Password, string operatorType, string rechargeType, string agentTransId, object userState) {
            if ((this.RechargeOperationCompleted == null)) {
                this.RechargeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRechargeOperationCompleted);
            }
            this.InvokeAsync("Recharge", new object[] {
                        OpCode,
                        custMobNo,
                        rchAmt,
                        clientMobileNo,
                        Password,
                        operatorType,
                        rechargeType,
                        agentTransId}, this.RechargeOperationCompleted, userState);
        }
        
        private void OnRechargeOperationCompleted(object arg) {
            if ((this.RechargeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RechargeCompleted(this, new RechargeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/checkWallet", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string checkWallet(string clientMobileNo, string Password) {
            object[] results = this.Invoke("checkWallet", new object[] {
                        clientMobileNo,
                        Password});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void checkWalletAsync(string clientMobileNo, string Password) {
            this.checkWalletAsync(clientMobileNo, Password, null);
        }
        
        /// <remarks/>
        public void checkWalletAsync(string clientMobileNo, string Password, object userState) {
            if ((this.checkWalletOperationCompleted == null)) {
                this.checkWalletOperationCompleted = new System.Threading.SendOrPostCallback(this.OncheckWalletOperationCompleted);
            }
            this.InvokeAsync("checkWallet", new object[] {
                        clientMobileNo,
                        Password}, this.checkWalletOperationCompleted, userState);
        }
        
        private void OncheckWalletOperationCompleted(object arg) {
            if ((this.checkWalletCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.checkWalletCompleted(this, new checkWalletCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/getTransactionStatus", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string getTransactionStatus(string clientMobileNo, string Password, string agentTransId) {
            object[] results = this.Invoke("getTransactionStatus", new object[] {
                        clientMobileNo,
                        Password,
                        agentTransId});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getTransactionStatusAsync(string clientMobileNo, string Password, string agentTransId) {
            this.getTransactionStatusAsync(clientMobileNo, Password, agentTransId, null);
        }
        
        /// <remarks/>
        public void getTransactionStatusAsync(string clientMobileNo, string Password, string agentTransId, object userState) {
            if ((this.getTransactionStatusOperationCompleted == null)) {
                this.getTransactionStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetTransactionStatusOperationCompleted);
            }
            this.InvokeAsync("getTransactionStatus", new object[] {
                        clientMobileNo,
                        Password,
                        agentTransId}, this.getTransactionStatusOperationCompleted, userState);
        }
        
        private void OngetTransactionStatusOperationCompleted(object arg) {
            if ((this.getTransactionStatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getTransactionStatusCompleted(this, new getTransactionStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void RechargeCompletedEventHandler(object sender, RechargeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RechargeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RechargeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void checkWalletCompletedEventHandler(object sender, checkWalletCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class checkWalletCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal checkWalletCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void getTransactionStatusCompletedEventHandler(object sender, getTransactionStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getTransactionStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getTransactionStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591